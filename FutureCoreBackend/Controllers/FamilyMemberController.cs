using AutoMapper;
using FutureCoreBackend.DTO.ResponseDTO;
using FutureCoreBackend.DTO;
using FutureCoreBackend.Helper;
using FutureCoreBackend.Models;
using FutureCoreBackend.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FutureCoreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamilyMemberController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;


        public FamilyMemberController(IMapper _mapper, IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }

        [HttpPost("AddFamilyMember")]
        [SwaggerOperation(
            Summary = "Add a new family member to an employee",
            Description = "This endpoint is used to add a new family member to the system. It requires the family member details in the form of a family member dto."
        )]
        public async Task<ResultDTO> AddFamilyMember(FamilyMemberDTO model)
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

                var familyMember = mapper.Map<FamilyMember>(model);
                familyMember.CreatedAt = DateTime.UtcNow;

                await unitOfWork.FamilyMemberRepo.Add(familyMember);
                await unitOfWork.CommitAsync();

                result.Statescode = StatusCodes.Status201Created;
                result.Message = "The family member created successfully.";
                result.Data = familyMember;
                result.Count = 1;
            }
            catch (Exception ex)
            {
                result.Statescode = StatusCodes.Status500InternalServerError;
                result.Message = $"Error adding family member {ex.Message}.";
            }
            return result;
        }

        [HttpPost("PaginatedFamilyMembers")]
        [SwaggerOperation(
            Summary = "Get a paginated list of family members",
            Description = "Returns a paginated list of family members based on the specified page index and page size."
        )]
        public async Task<ResultDTO> GetFamilyMembersPaginated(int pageIndex = 1, int pageSize = 10)
        {
            var result = new ResultDTO();

            try
            {
                var familyMembers = unitOfWork.FamilyMemberRepository.GetAllWithInclude();

                var paginatedList = await unitOfWork.FamilyMemberRepo.GetAllPaginated(familyMembers, pageIndex, pageSize);
                var paginatedResponse = new PaginatedList<FamilyMemberResponseDTO>()
                {
                    Items = mapper.Map<List<FamilyMemberResponseDTO>>(paginatedList.Items),
                    PageCount = paginatedList.PageCount,
                    PageIndex = pageIndex,
                    Count = paginatedList.Count
                }; 

                result.Statescode = StatusCodes.Status200OK;
                result.Message = "Family members retrevied successfully.";
                result.Data = paginatedResponse;
                result.Count = paginatedResponse.Count;
            }
            catch (Exception ex)
            {
                result.Statescode = StatusCodes.Status500InternalServerError;
                result.Message = $"Error retreving family members {ex.Message}.";
            }
            return result;
        }

        [HttpGet("GetAll")]
        [SwaggerOperation(
            Summary = "Get all family members",
            Description = "Fetches a complete list of all family members from the database."
        )]
        public async Task<ResultDTO> GetAllFamilyMember()
        {
            var result = new ResultDTO();
            try
            {
                var familyMembers = unitOfWork.FamilyMemberRepository.GetAllWithInclude();


                result.Statescode = StatusCodes.Status200OK;
                result.Message = "Family members retrevied successfully.";
                result.Data = mapper.Map<List<FamilyMemberResponseDTO>>(familyMembers);
                result.Count = familyMembers.Count;
            }
            catch (Exception ex)
            {
                result.Statescode = StatusCodes.Status500InternalServerError;
                result.Message = $"Error retreving family members {ex.Message}.";
            }
            return result;
        }

        [HttpGet("GetByID")]
        [SwaggerOperation(
            Summary = "Get a family member by id",
            Description = "Fetches a family member object by its id."
        )]
        public async Task<ResultDTO> GetFamilyMemberByID(int ID)
        {
            var result = new ResultDTO();
            try
            {
                var familyMember = await unitOfWork.FamilyMemberRepo.GetByID(ID);

                if (familyMember == null)
                {
                    result.Statescode = StatusCodes.Status404NotFound;
                    result.Message = "Family member not found.";
                    return result;
                }

                result.Statescode = StatusCodes.Status200OK;
                result.Message = "Family member retrevied successfully.";
                result.Data = mapper.Map<FamilyMemberDTO>(familyMember);
                result.Count = 1;
            }
            catch (Exception ex)
            {
                result.Statescode = StatusCodes.Status500InternalServerError;
                result.Message = $"Error retreving family member {ex.Message}.";
            }
            return result;
        }

        [HttpGet("GetByName")]
        [SwaggerOperation(
            Summary = "Get a family member by name",
            Description = "Fetches an family member object by its name."
        )]
        public async Task<ResultDTO> GetFamilyMemberByName(string Name)
        {
            var result = new ResultDTO();
            try
            {
                var familyMembers = unitOfWork.FamilyMemberRepository.GetAllWithInclude()
                    .Where(e => e.Name.ToLower().Contains(Name.ToLower()));

                if (!familyMembers.Any())
                {
                    result.Statescode = StatusCodes.Status404NotFound;
                    result.Message = "Family member not found.";
                    return result;
                }

                result.Statescode = StatusCodes.Status200OK;
                result.Message = "Family member retrevied successfully.";
                result.Data = mapper.Map<List<FamilyMemberResponseDTO>>(familyMembers);
                result.Count = familyMembers.Count();
            }
            catch (Exception ex)
            {
                result.Statescode = StatusCodes.Status500InternalServerError;
                result.Message = $"Error retreving family member {ex.Message}.";
            }
            return result;
        }

        [HttpGet("GetByEmployee")]
        [SwaggerOperation(
            Summary = "Get a family members by employee",
            Description = "Fetches an family members by its employee."
        )]
        public async Task<ResultDTO> GetFamilyMemberByEmployee(int employeeId)
        {
            var result = new ResultDTO();
            try
            {
                var familyMembers = (await unitOfWork.FamilyMemberRepo.GetAllAsync())
                    .Where(e => e.EmployeeId == employeeId);

                if (!familyMembers.Any())
                {
                    result.Statescode = StatusCodes.Status404NotFound;
                    result.Message = "Family member not found.";
                    return result;
                }

                result.Statescode = StatusCodes.Status200OK;
                result.Message = "Family member retrevied successfully.";
                result.Data = mapper.Map<List<FamilyMember>>(familyMembers);
                result.Count = familyMembers.Count();
            }
            catch (Exception ex)
            {
                result.Statescode = StatusCodes.Status500InternalServerError;
                result.Message = $"Error retreving family member {ex.Message}.";
            }
            return result;
        }

        [HttpPut("EditFamilyMember")]
        [SwaggerOperation(
            Summary = "Update the family member",
            Description = "Edit the data of the family member."
        )]
        public async Task<ResultDTO> UpdateFamilyMember(int ID, FamilyMemberDTO newModel)
        {
            var result = new ResultDTO();

            try
            {
                var oldFamilyMember = await unitOfWork.FamilyMemberRepo.GetByID(ID);
                if (oldFamilyMember == null)
                {
                    result.Statescode = StatusCodes.Status404NotFound;
                    result.Message = "The family member not found.";
                    result.Data = ID;
                    return result;
                }

                mapper.Map(newModel, oldFamilyMember);
                oldFamilyMember.UpdatedAt = DateTime.Now;

                unitOfWork.FamilyMemberRepo.Update(oldFamilyMember);
                await unitOfWork.CommitAsync();

                result.Statescode = StatusCodes.Status200OK;
                result.Message = "The family member updated successfully.";
                result.Data = oldFamilyMember;
            }
            catch (Exception ex)
            {
                result.Statescode = StatusCodes.Status500InternalServerError;
                result.Message = $"Error updating family member {ex.Message}.";
            }
            return result;
        }

        [HttpDelete("DeleteFamilyMember")]
        [SwaggerOperation(
            Summary = "Delete the family member",
            Description = "Delete the family member."
        )]
        public async Task<ResultDTO> DeleteFamilyMember(int familyMemberId)
        {
            var result = new ResultDTO();

            try
            {
                var familyMember = await unitOfWork.FamilyMemberRepo.GetByID(familyMemberId);
                if (familyMemberId == null)
                {
                    result.Statescode = StatusCodes.Status404NotFound;
                    result.Message = "The family member not found.";
                    result.Data = familyMemberId;
                    return result;
                }

                unitOfWork.FamilyMemberRepo.Delete(familyMember);
                await unitOfWork.CommitAsync();

                result.Statescode = StatusCodes.Status200OK;
                result.Message = "The family member deleted successfully.";
                result.Data = null;
            }
            catch (Exception ex)
            {
                result.Statescode = StatusCodes.Status500InternalServerError;
                result.Message = $"Error deleting family member {ex.Message}.";
            }
            return result;
        }

        [HttpPut("RemoveFamilyMember")]
        [SwaggerOperation(
            Summary = "Remove the family member",
            Description = "Remove the family member."
        )]
        public async Task<ResultDTO> RemoveFamilyMember(int familyMemberId)
        {
            var result = new ResultDTO();

            try
            {
                var familyMember = await unitOfWork.FamilyMemberRepo.GetByID(familyMemberId);
                if (familyMember == null)
                {
                    result.Statescode = StatusCodes.Status404NotFound;
                    result.Message = "The family member not found.";
                    result.Data = familyMemberId;
                    return result;
                }

                unitOfWork.FamilyMemberRepo.SoftDelete(familyMember);
                familyMember.UpdatedAt = DateTime.Now;
                unitOfWork.FamilyMemberRepo.Update(familyMember);
                await unitOfWork.CommitAsync();

                result.Statescode = StatusCodes.Status200OK;
                result.Message = "The family member deleted successfully.";
                result.Data = null;
            }
            catch (Exception ex)
            {
                result.Statescode = StatusCodes.Status500InternalServerError;
                result.Message = $"Error deleting family member {ex.Message}.";
            }
            return result;
        }
    }
}
