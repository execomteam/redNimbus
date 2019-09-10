using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedNimbus.API.Controllers;
using RedNimbus.API.Services;
using RedNimbus.API.Services.Interfaces;
using RedNimbus.Either;
using RedNimbus.Either.Errors;

namespace API.Controllers
{
    [Route("api/lambda")]
    [ApiController]
    public class LambdaController : BaseController
    {
        public ILambdaService _lambdaService;
        public LambdaController(ILambdaService lambdaService)
        {
            _lambdaService = lambdaService;
        }

        //call with ...api/lambda/create
        [HttpPost("create")]
        public IActionResult Create(CreateLambdaDto dto)
        {
            return _lambdaService.CreateLambda(dto)
                .Map(() => AllOk())
                .Reduce(NotFoundErrorHandler, e => e is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }
    }
}