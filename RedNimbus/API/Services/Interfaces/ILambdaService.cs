using RedNimbus.Either;
using RedNimbus.Either.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.API.Services.Interfaces
{
    public interface ILambdaService
    {
        Either<IError, CreateLambdaDto> CreateLambda(CreateLambdaDto createlambda);
    }
}
