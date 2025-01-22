using AutoMapper;
using FutureCoreBackend.DTO;
using FutureCoreBackend.DTO.ResponseDTO;
using FutureCoreBackend.Helper;
using FutureCoreBackend.Models;

namespace FutureCoreBackend.AutoMapper
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.NationalId))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ReverseMap();
            
            CreateMap<Employee, EmployeeResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.NationalId))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => NationalHelper.CalculateAge(src.BirthDate)))
                .ReverseMap();
        }
    }
}
