using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedNimbus.Either.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.API.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult AllOk()
        {
            return Ok(new Empty());
        }

        protected IActionResult AllOk(object obj)
        {
            return Ok(obj);
        }

        protected IActionResult BadRequestErrorHandler(IError error)
        {
            return BadRequest(error);
        }

        protected IActionResult InternalServisErrorHandler(IError error)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, error);
        }

        protected IActionResult NotFoundErrorHandler(IError error)
        {
            return NotFound(error);
        }

        protected IActionResult AuthenticationErrorHandler(IError error)
        {
            return StatusCode(StatusCodes.Status406NotAcceptable, error);
        }

    }
}
