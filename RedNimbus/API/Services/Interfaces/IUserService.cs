using RedNimbus.Domain;
using RedNimbus.Either;
using RedNimbus.Either.Errors;

namespace RedNimbus.API.Services.Interfaces
{
    public interface IUserService
    {
        Either<IError, User> RegisterUser(User user);
    }
}
