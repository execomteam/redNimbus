using RedNimbus.DTO;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using RedNimbus.UserService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.NewUserService.Services.Interfaces
{
    public interface IUserService
    {
        Either<IError, User> Create(User user);
        Either<IError, User> Authenticate(User user);
        UserDto AddAuthenticatedUser(UserDto user);
        Either<IError, User> GetUserByToken(string token);
    }
}
