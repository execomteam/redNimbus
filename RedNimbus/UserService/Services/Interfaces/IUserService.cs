using RedNimbus.DTO;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using RedNimbus.UserService.Model;

namespace RedNimbus.UserService.Services.Interfaces
{
    public interface IUserService
    {
        Either<IError,User> Create(User user);
        Either<IError, User> Authenticate(User user);
        UserDto AddAuthenticatedUser(UserDto user);
        User GetUserByToken(string token);
    }
}
