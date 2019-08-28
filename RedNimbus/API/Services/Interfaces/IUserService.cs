using RedNimbus.DTO;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.API.Services.Interfaces
{
    public interface IUserService
    {
        Either<IError, TSuccess> RegisterUser<TRequest, TSuccess>(CreateUserDto createUserDto);
    }
}
