using AutoMapper;
using FutureCoreBackend.DTO;
using FutureCoreBackend.Helper;
using FutureCoreBackend.Models;

namespace FutureCoreBackend.AutoMapper
{
    public class FamilyMemberProfile : Profile
    {
        public FamilyMemberProfile()
        {
            CreateMap<FamilyMember, FamilyMemberDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.Relationship, opt => opt.MapFrom(src => src.Relationship))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ReverseMap();
            
            CreateMap<FamilyMember, FamilyMemberResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.Relationship, opt => opt.MapFrom(src => src.Relationship))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FullName))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => NationalHelper.CalculateAge(src.BirthDate)))
                .ReverseMap();
        }
    }
}
