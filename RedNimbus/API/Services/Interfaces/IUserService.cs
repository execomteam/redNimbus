using RedNimbus.DTO;
using RedNimbus.Either;
using RedNimbus.Either.Errors;

namespace RedNimbus.API.Services.Interfaces
{
    public interface IUserService
    {
        Either<IError, TSuccess> RegisterUser<TRequest, TSuccess>(CreateUserDto createUserDto);

        Either<IError, TSuccess> AuthenticateUser<TRequest, TSuccess>(AuthenticateUserDto authenticateUserDto);
    }
}
