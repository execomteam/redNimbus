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
    public class UserController : ControllerBase
    {
        private readonly ICommunicationService _communicationService;

        public UserController(ICommunicationService communicationService)
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
        /*
        [HttpPost]
        public IActionResult RegisterUser([FromBody]CreateUserDto createUserDto)
        {
            string topic = "RegisterUser";

            // - Dodeliti topic
            // - Konvertovati DTO u nas protobuf message
            // - Od toga napraviti NetMQMessage
            // - Poslati preko socket factory zahtev
            // - kad stigne response uraditi nesto
        }
        */
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthorizeUserDto userLoginDTO)
        {
            return _communicationService.Send<AuthorizeUserDto, UserDto>("api/user/authenticate", userLoginDTO)
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