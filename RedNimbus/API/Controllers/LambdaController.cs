using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTO;
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
                .Map((id) => AllOk(id)) //return lambda id
                .Reduce(NotFoundErrorHandler, e => e is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }

        //note:
        //return value of lambda MUST BE some object that can be
        //coverted in JSON obj
        [HttpGet("{lambdaId}")]
        public IActionResult Get([FromRoute] string lambdaId)
        {
            return _lambdaService.GetLambda(lambdaId, Request.Headers["token"])
                 .Map((r) => AllOk(r)) //if ok return result
                 .Reduce(NotFoundErrorHandler, e => e is NotFoundError)
                 .Reduce(InternalServisErrorHandler);
        }
    }
}