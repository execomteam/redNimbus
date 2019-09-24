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
using RedNimbus.Either.Mappings;

namespace API.Controllers
{
    [Route("api/lambda")]
    [ApiController]
    public class LambdaController : BaseController
    {
        private readonly ILambdaService _lambdaService;

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
            Guid requestId = Guid.NewGuid();

            return _lambdaService.Create(dto, Request.Headers["token"], requestId)
                .Map((id) => AllOk(id))
                .Reduce(NotFoundErrorHandler, e => e is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }

        /// <summary>
        /// Endpoint for retreiving the set of all lambdas the user has created.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            Guid requestId = Guid.NewGuid();

            return _lambdaService.GetAll(Request.Headers["token"], requestId)
                 .Map((r) => AllOk(r))
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
        public IActionResult ExecuteGetLambda([FromRoute] string lambdaId)
        {
            Guid requestId = Guid.NewGuid();

            return _lambdaService.ExecuteGetLambda(lambdaId, Request.Headers["token"], requestId)
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
        public IActionResult ExecutePostLambda([FromRoute] string lambdaId,[FromForm] IFormFile data)
        {
            Guid requestId = Guid.NewGuid();

            return _lambdaService.ExecutePostLambda(lambdaId, data, requestId)
                 .Map((x) => (IActionResult)File(x, "application/octet-stream"))
                 .Reduce(NotFoundErrorHandler, e => e is NotFoundError)
                 .Reduce(InternalServisErrorHandler);
        }
    }
}