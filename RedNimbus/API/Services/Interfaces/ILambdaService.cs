﻿using DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
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
        Either<IError, CreateLambdaDto> CreateLambda(CreateLambdaDto createLambdaDto);
        Either<IError, string> GetLambda(string lambdaId);
        Either<IError, PostLambdaDto> PostLambda(string lambdaId, IFormFile data);
    }
}
