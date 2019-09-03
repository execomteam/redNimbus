﻿using AutoMapper;
using RedNimbus.UserService.Model;
using RedNimbus.DTO;
using UserService.DatabaseModel;

namespace RedNimbus.UserService.Mappings
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
            CreateMap<UserDto, KeyDto>();
            CreateMap<KeyDto, UserDto>();
            CreateMap<User, UserDB>();
            CreateMap<UserDB, User>();
        }
    }
}
