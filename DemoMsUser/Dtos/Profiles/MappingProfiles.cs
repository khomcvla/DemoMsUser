using AutoMapper;
using DemoMsUser.Data.Models;

namespace DemoMsUser.Dtos.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            //----------------------------------
            CreateMap<User, UserGetDto>();
            CreateMap<User, UserGetShortDto>();
            CreateMap<User, UserPatchDto>();
            CreateMap<User, UserPostDto>();

            //----------------------------------
            CreateMap<UserGetDto, UserGetShortDto>();

            //----------------------------------
            CreateMap<UserPostDto, User>()
                .ForAllMembers(opt => opt.UseDestinationValue());

            CreateMap<UserPatchDto, User>()
                .ForAllMembers(opt => opt.UseDestinationValue());

            //----------------------------------
            CreateMap<UserPatchDtoAdmin, User>()
                .ForAllMembers(opt => opt.UseDestinationValue());
        }
    }
}
