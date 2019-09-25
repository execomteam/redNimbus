using DTO;
using Microsoft.AspNetCore.Http;
using RedNimbus.Domain;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using System;
using System.Collections.Generic;

namespace RedNimbus.API.Services.Interfaces
{
    public interface ILambdaService
    {
        Either<IError, CreateLambdaDto> Create(CreateLambdaDto createLambdaDto, string token, Guid requestId);

        Either<IError, List<Lambda>> GetAll(string token, Guid requestId);

        Either<IError, string> ExecuteGetLambda(string lambdaId, string token, Guid requestId);

        Either<IError, byte[]> ExecutePostLambda(string lambdaId, IFormFile data, Guid requestId);
    }
}
