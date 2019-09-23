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
        /// <summary>
        /// Endpoint for creating lambda functions.
        /// </summary>
        /// <param name="dto">DTO that contains all values required for lambda function creation.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create([FromForm]CreateLambdaDto dto)
        {
            return _lambdaService.CreateLambda(dto)
                .Map((id) => AllOk(id))
                .Reduce(NotFoundErrorHandler, e => e is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }

        /// <summary>
        /// Endpoint for executing a lambda function using GET request.
        /// Return value of the lambda function MUST BE some object that can be converted in JSON format.
        /// </summary>
        /// <param name="lambdaId">Unique identifier of the lambda function.</param>
        /// <returns></returns>
        [HttpGet("{lambdaId}")]
        public IActionResult Get([FromRoute] string lambdaId)
        {
            return _lambdaService.GetLambda(lambdaId)
                 .Map((r) => AllOk(r))
                 .Reduce(NotFoundErrorHandler, e => e is NotFoundError)
                 .Reduce(InternalServisErrorHandler);
        }

        /// <summary>
        /// Endpoint for executing a lambda function using POST request.
        /// </summary>
        /// <param name="lambdaId">Unique identifier of the lambda function.</param>
        /// <param name="data">POST request data.</param>
        /// <returns></returns>
        [HttpPost("{lambdaId}")]
        public IActionResult Post([FromRoute] string lambdaId,[FromForm] IFormFile data)
        {
            return _lambdaService.PostLambda(lambdaId, data)
                 .Map((r) => AllOk(r))
                 .Reduce(NotFoundErrorHandler, e => e is NotFoundError)
                 .Reduce(InternalServisErrorHandler);
        }
    }
}