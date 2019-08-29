using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedNimbus.API.Models;
using RedNimbus.API.Services.Interfaces;
using RedNimbus.DTO;
using RedNimbus.Either;
using RedNimbus.Either.Errors;

namespace RedNimbus.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class RestUserController : ControllerBase
    {
        private readonly IRestUserService _communicationService;

        public RestUserController(IRestUserService communicationService)
        {
            _communicationService = communicationService;
        }

        #region IActionResult

        private IActionResult AllOk()
        {
            return Ok(new Empty());
        }

        private IActionResult AllOk(object obj)
        {
            return Ok(obj);
        }

        private IActionResult BadRequestErrorHandler(IError error)
        {
            return BadRequest(error.Message);
        }

        private IActionResult InternalServisErrorHandler(IError error)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, error.Message);
        }

        private IActionResult NotFoundErrorHandler(IError error)
        {
            return NotFound(error.Message);
        }

        private IActionResult AuthenticationErrorHandler(IError error)
        {
            return StatusCode(StatusCodes.Status406NotAcceptable, error.Message);
        }

        #endregion

        [HttpPost]
        public IActionResult Post([FromBody]CreateUserDto createUserDto)
        {
            return _communicationService.Send<CreateUserDto, Empty>("api/user", createUserDto)
                 .Result
                 .Map(() => AllOk())
                 .Reduce(BadRequestErrorHandler, x => x is FormatError)
                 .Reduce(InternalServisErrorHandler);
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateUserDto userLoginDTO)
        {
            return _communicationService.Send<AuthenticateUserDto, UserDto>("api/user/authenticate", userLoginDTO)
                .Result
                .Map(x => AllOk(x))
                .Reduce(AuthenticationErrorHandler, err => err is AuthenticationError)
                .Reduce(InternalServisErrorHandler);
        }

        [HttpPost("get")]
        public IActionResult GetUser([FromBody]KeyDto keyData)
        {
            return _communicationService.Send<KeyDto, UserDto>("api/user/get", keyData)
                .Result
                .Map(x => AllOk(x))
                .Reduce(NotFoundErrorHandler, err => err is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }
    }
}