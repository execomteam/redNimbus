using DTO;
using Microsoft.Extensions.Primitives;
using RedNimbus.Domain;
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
        Either<IError, CreateLambdaDto> CreateLambda(CreateLambdaDto createlambda, string token, Guid requestId);
        Either<IError, string> GetLambda(string lambdaId, string token, Guid requestId);

        Either<IError, List<Lambda>> GetLambdas(string token, Guid requestId);
    }
}
