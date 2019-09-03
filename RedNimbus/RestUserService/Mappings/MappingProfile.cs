using AutoMapper;
using RedNimbus.Domain;
using RedNimbus.DTO;
using UserService.DatabaseModel;

namespace RedNimbus.RestUserService.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<User, CreateUserDto>();
            CreateMap<CreateUserDto, User>();
            CreateMap<User, AuthenticateUserDto>();
            CreateMap<AuthenticateUserDto, User>();
            CreateMap<UserDto, KeyDto>();
            CreateMap<KeyDto, UserDto>();
            CreateMap<User, UserDB>();
            CreateMap<UserDB, User>();
        }
    }
}
