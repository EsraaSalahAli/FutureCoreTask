using AutoMapper;
using FutureCoreBackend.DTO;
using FutureCoreBackend.DTO.ResponseDTO;
using FutureCoreBackend.Helper;
using FutureCoreBackend.Models;
using FutureCoreBackend.Repository;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace FutureCoreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpolyeeController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;


        public EmpolyeeController(IMapper _mapper, IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }

        [HttpPost("AddEmployee")]
        [SwaggerOperation(
            Summary = "Add a new employee",
            Description = "This endpoint is used to add a new employee to the system. It requires the employee details in the form of a employee dto."
        )]
        public async Task<ResultDTO> AddEmployee(EmployeeDTO model)
        {
            var result = new ResultDTO();
            try
            {
                if (!ModelState.IsValid)
                {
                    result.Statescode = StatusCodes.Status400BadRequest;
                    result.Message = "The data is invalid.";
                    result.Data = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return result;
                }

                var employee = mapper.Map<Employee>(model);
                employee.CreatedAt = DateTime.UtcNow;

                var birthdate = NationalHelper.ExtractBirthdate(employee.NationalId);
                if(birthdate == null)
                {
                    result.Statescode = StatusCodes.Status400BadRequest;
                    result.Message = "Invalid National ID: Could not extract a valid birthdate.";
                    return result;
                }

                employee.BirthDate = birthdate.Value;

                await unitOfWork.EmployeeRepo.Add(employee);
                await unitOfWork.CommitAsync();

                result.Statescode = StatusCodes.Status201Created;
                result.Message = "The employee created successfully.";
                result.Data = mapper.Map<EmployeeResponseDTO>(employee);
                result.Count = 1;
            }
            catch (Exception ex)
            {
                result.Statescode = StatusCodes.Status500InternalServerError;
                result.Message = $"Error adding employee {ex.Message}.";
            }
            return result;
        }

        [HttpPost("PaginatedEmployee")]
        [SwaggerOperation(
            Summary = "Get a paginated list of employees", 
            Description = "Returns a paginated list of employees based on the specified page index and page size."
        )]
        public async Task<ResultDTO> GetEmployeesPaginated(int pageIndex = 1, int pageSize = 10)
        {
            var result = new ResultDTO();

            try
            {
                var emplyees = await unitOfWork.EmployeeRepo.GetAllAsync();

                var paginatedList = await unitOfWork.EmployeeRepo.GetAllPaginated(emplyees, pageIndex, pageSize);
                var paginatedResponse = new PaginatedList<EmployeeResponseDTO>()
                {
                    Items = mapper.Map<List<EmployeeResponseDTO>>(paginatedList.Items),
                    PageCount = paginatedList.PageCount,
                    PageIndex = pageIndex,
                    Count = paginatedList.Count
                };

                result.Statescode = StatusCodes.Status200OK;
                result.Message = "Employees retrevied successfully.";
                result.Data = paginatedResponse;
                result.Count = paginatedResponse.Count;
            }
            catch (Exception ex)
            {
                result.Statescode = StatusCodes.Status500InternalServerError;
                result.Message = $"Error retreving employees {ex.Message}.";
            }
            return result;
        }

        [HttpGet("GetAll")]
        [SwaggerOperation(
            Summary = "Get all employees",
            Description = "Fetches a complete list of all employees from the database."
        )]
        public async Task<ResultDTO> GetAllEmployees()
        {
            var result = new ResultDTO();
            try
            {
                var employees = await unitOfWork.EmployeeRepo.GetAllAsync();


                result.Statescode = StatusCodes.Status200OK;
                result.Message = "Employees retrevied successfully.";
                result.Data = mapper.Map<List<EmployeeResponseDTO>>(employees);
                result.Count = employees.Count;
            }
            catch (Exception ex)
            {
                result.Statescode = StatusCodes.Status500InternalServerError;
                result.Message = $"Error retreving employees {ex.Message}.";
            }
            return result;
        }

        [HttpGet("GetByID")]
        [SwaggerOperation(
            Summary = "Get a employee by id",
            Description = "Fetches a employee object by its id."
        )]
        public async Task<ResultDTO> GetEmployeeByID(int ID)
        {
            var result = new ResultDTO();
            try
            {
                var employee = await unitOfWork.EmployeeRepo.GetByID(ID);

                if (employee == null)
                {
                    result.Statescode = StatusCodes.Status404NotFound;
                    result.Message = "Employee not found.";
                    return result;
                }

                result.Statescode = StatusCodes.Status200OK;
                result.Message = "Employee retrevied successfully.";
                result.Data = mapper.Map<EmployeeResponseDTO>(employee);
                result.Count = 1;
            }
            catch (Exception ex)
            {
                result.Statescode = StatusCodes.Status500InternalServerError;
                result.Message = $"Error retreving employee {ex.Message}.";
            }
            return result;
        }

        [HttpGet("GetByName")]
        [SwaggerOperation(
            Summary = "Get a employee by name",
            Description = "Fetches an employee object by its name."
        )]
        public async Task<ResultDTO> GetEmloyeeByName(string Name)
        {
            var result = new ResultDTO();
            try
            {
                var employees = (await unitOfWork.EmployeeRepo.GetAllAsync())
                    .Where(e => e.FullName.ToLower().Contains(Name.ToLower()));

                if (!employees.Any())
                {
                    result.Statescode = StatusCodes.Status404NotFound;
                    result.Message = "Employee not found.";
                    return result;
                }

                result.Statescode = StatusCodes.Status200OK;
                result.Message = "Emplyees retrevied successfully.";
                result.Data = mapper.Map<List<EmployeeResponseDTO>>(employees);
                result.Count = employees.Count();
            }
            catch (Exception ex)
            {
                result.Statescode = StatusCodes.Status500InternalServerError;
                result.Message = $"Error retreving employee {ex.Message}.";
            }
            return result;
        }

        [HttpPut("EditEmployee")]
        [SwaggerOperation(
            Summary = "Update the employee",
            Description = "Edit the data of the employee."
        )]
        public async Task<ResultDTO> UpdateEmployee(int ID, EmployeeDTO newModel)
        {
            var result = new ResultDTO();

            try
            {
                var oldEmployee = await unitOfWork.EmployeeRepo.GetByID(ID);
                if (oldEmployee == null)
                {
                    result.Statescode = StatusCodes.Status404NotFound;
                    result.Message = "The employee not found.";
                    result.Data = ID;
                    return result;
                }

                mapper.Map(newModel, oldEmployee);
                oldEmployee.UpdatedAt = DateTime.Now;
                var birthdate = NationalHelper.ExtractBirthdate(newModel.NationalId);
                if (birthdate == null)
                {
                    result.Statescode = StatusCodes.Status400BadRequest;
                    result.Message = "Invalid National ID: Could not extract a valid birthdate.";
                    return result;
                }

                oldEmployee.BirthDate = birthdate.Value;

                unitOfWork.EmployeeRepo.Update(oldEmployee);

                await unitOfWork.CommitAsync();

                result.Statescode = StatusCodes.Status200OK;
                result.Message = "The employee updated successfully.";
                result.Data = oldEmployee;
            }
            catch (Exception ex)
            {
                result.Statescode = StatusCodes.Status500InternalServerError;
                result.Message = $"Error updating employee {ex.Message}.";
            }
            return result;
        }

        [HttpDelete("DeleteEmployee")]
        [SwaggerOperation(
            Summary = "Delete the employee",
            Description = "Delete the employee."
        )]
        public async Task<ResultDTO> DeleteEmployee(int employeeId)
        {
            var result = new ResultDTO();

            try
            {
                var employee = await unitOfWork.EmployeeRepo.GetByID(employeeId);
                if (employee == null)
                {
                    result.Statescode = StatusCodes.Status404NotFound;
                    result.Message = "The employee not found.";
                    result.Data = employeeId;
                    return result;
                }

                unitOfWork.EmployeeRepo.Delete(employee);

                var employeeFamilyMembers = (await unitOfWork.FamilyMemberRepo.GetAllAsync())
                    .Where(fm => fm.EmployeeId == employeeId);

                if (employeeFamilyMembers.Any())
                {
                    foreach (var familyMember in employeeFamilyMembers)
                    {
                        unitOfWork.FamilyMemberRepo.Delete(familyMember);
                    }
                }

                await unitOfWork.CommitAsync();

                result.Statescode = StatusCodes.Status200OK;
                result.Message = "The employee deleted successfully.";
                result.Data = null;
            }
            catch (Exception ex)
            {
                result.Statescode = StatusCodes.Status500InternalServerError;
                result.Message = $"Error deleting employee {ex.Message}.";
            }
            return result;
        }

        [HttpPut("RemoveEmployee")]
        [SwaggerOperation(
            Summary = "Remove the employee",
            Description = "Remove the employee."
        )]
        public async Task<ResultDTO> RemoveEmployee(int employeeId)
        {
            var result = new ResultDTO();

            try
            {
                var employee = await unitOfWork.EmployeeRepo.GetByID(employeeId);
                if (employee == null)
                {
                    result.Statescode = StatusCodes.Status404NotFound;
                    result.Message = "The employee not found.";
                    result.Data = employeeId;
                    return result;
                }

                unitOfWork.EmployeeRepo.SoftDelete(employee);
                employee.UpdatedAt = DateTime.Now;
                unitOfWork.EmployeeRepo.Update(employee);

                var employeeFamilyMembers = (await unitOfWork.FamilyMemberRepo.GetAllAsync())
                    .Where(fm => fm.EmployeeId == employeeId);

                if (employeeFamilyMembers.Any())
                {
                    foreach (var familyMember in employeeFamilyMembers)
                    {
                        unitOfWork.FamilyMemberRepo.SoftDelete(familyMember);
                    }
                }
                await unitOfWork.CommitAsync();

                result.Statescode = StatusCodes.Status200OK;
                result.Message = "The employee deleted successfully.";
                result.Data = null;
            }
            catch (Exception ex)
            {
                result.Statescode = StatusCodes.Status500InternalServerError;
                result.Message = $"Error deleting employee {ex.Message}.";
            }
            return result;
        }
    }
}
