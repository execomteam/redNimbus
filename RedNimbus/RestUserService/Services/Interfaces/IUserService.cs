using RedNimbus.DTO;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using RedNimbus.Domain;

namespace RedNimbus.RestUserService.Services.Interfaces
{
    public interface IUserService
    {
        Either<IError,User> Create(User user);
        Either<IError, User> Authenticate(User user);
        UserDto AddAuthenticatedUser(UserDto user);
        Either<IError,User> GetUserByToken(string token);
    }
}
