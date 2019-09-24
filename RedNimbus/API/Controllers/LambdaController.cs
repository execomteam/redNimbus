﻿using System;
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
    [System.Runtime.InteropServices.Guid("13CFBDF0-499A-40A8-966F-F74B38407A24")]
    public class LambdaController : BaseController
    {
        public ILambdaService _lambdaService;
        public LambdaController(ILambdaService lambdaService)
        {
            _lambdaService = lambdaService;
        }

        [HttpPost("create")]
        public IActionResult Create([FromForm]CreateLambdaDto dto)
        {
            Guid requestId = Guid.NewGuid();

            return _lambdaService.CreateLambda(dto, Request.Headers["token"], requestId)
                .Map((id) => AllOk(id)) //return lambda id
                .Reduce(NotFoundErrorHandler, e => e is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }

        [HttpGet("getLambdas")]
        public IActionResult GetLambdas()
        {
            Guid requestId = Guid.NewGuid();

            return _lambdaService.GetLambdas(Request.Headers["token"], requestId)
                 .Map((r) => AllOk(r))
                 .Reduce(NotFoundErrorHandler, e => e is NotFoundError)
                 .Reduce(InternalServisErrorHandler);
        }

        /// <summary>
        /// return value of lambda MUST BE some object that can be coverted in JSON obj
        /// </summary>
        /// <param name="lambdaId"></param>
        /// <returns></returns>
        [HttpGet("{lambdaId}")]
        public IActionResult Get([FromRoute] string lambdaId)
        {
            Guid requestId = Guid.NewGuid();

            return _lambdaService.GetLambda(lambdaId, Request.Headers["token"], requestId)
                 .Map((r) => AllOk(r)) //if ok return result
                 .Reduce(NotFoundErrorHandler, e => e is NotFoundError)
                 .Reduce(InternalServisErrorHandler);
        }
    }
}