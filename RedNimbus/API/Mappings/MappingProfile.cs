using AutoMapper;
using RedNimbus.API.Model;
using RedNimbus.API.DTO;

namespace RedNimbus.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<User, CreateUserDto>();
            CreateMap<CreateUserDto, User>();
            CreateMap<User, AuthorizeUserDto>();
            CreateMap<AuthorizeUserDto, User>();
        }
    }
}
