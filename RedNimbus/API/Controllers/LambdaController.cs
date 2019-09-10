using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedNimbus.API.Controllers;
using RedNimbus.API.Services;
using RedNimbus.Either;
using RedNimbus.Either.Errors;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LambdaController : BaseController
    {
        public LambdaService _lambdaService;
        public LambdaController(LambdaService lambdaService)
        {
            _lambdaService = lambdaService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return _lambdaService.GetLambda()
                .Map(() => AllOk())
                .Reduce(NotFoundErrorHandler, e => e is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }
    }
}