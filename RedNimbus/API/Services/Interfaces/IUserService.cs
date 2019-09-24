using RedNimbus.Domain;
using RedNimbus.DTO;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using System;

namespace RedNimbus.API.Services.Interfaces
{
    public interface IUserService
    {
        Either<IError, User> RegisterUser(User user, Guid id);
        Either<IError, KeyDto> Authenticate(User user, Guid id);
        Either<IError, User> GetUserByToken(string token, Guid id);
        Either<IError, Empty> deactivateUserAccount(string token);
        Either<IError, bool> EmailConfirmation(string token);
    }
}
