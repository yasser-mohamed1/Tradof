using AutoMapper;
using Tradof.Admin.Services.DataTransferObject.AuthenticationDto;
using Tradof.Data.Entities;
namespace Tradof.Admin.Services.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<RegisterAdminDto, ApplicationUser>()
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<ApplicationUser, GetUserDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserName)).ReverseMap();
        }
    }
}
