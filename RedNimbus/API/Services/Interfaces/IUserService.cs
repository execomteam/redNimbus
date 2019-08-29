using RedNimbus.Domain;
using RedNimbus.DTO;
using RedNimbus.Either;
using RedNimbus.Either.Errors;

namespace RedNimbus.API.Services.Interfaces
{
    public interface IUserService
    {
        Either<IError, User> RegisterUser(User user);
        Either<IError, KeyDto> Authenticate(User user);

        Either<IError, User> GetUserByToken(string token);
    }
}
