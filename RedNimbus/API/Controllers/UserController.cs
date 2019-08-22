using Either;
using Microsoft.AspNetCore.Mvc;
using RedNimbus.API.Models;
using RedNimbus.API.Services.Interfaces;
using RedNimbus.DTO;
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

        private IActionResult AllOk()
        {
            return Ok();
        }

        private IActionResult UnprocessableEntityErr(IError error)
        {
            return UnprocessableEntity(error.Message);
        }

        [HttpPost]
        public IActionResult Post([FromBody]CreateUserDto createUserDto)
        {
            // var response = _communicationService.Send<CreateUserDto, Response<Empty>>("api/user", createUserDto).Result;

            var a = _communicationService.Send<CreateUserDto, Empty>("api/user", createUserDto)
                 .Result;
            var b = a
                 .Map(AllOk);
            var c = b
                 .Reduce(x => UnprocessableEntityErr(x));
            return c;
            
        }




        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthorizeUserDto userLoginDTO)
        {
            /*var response = _communicationService.Send<AuthorizeUserDto, UserDto>("api/user/authenticate", userLoginDTO).Result; 
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    return Ok(response.Value);
                default:
                    return UnprocessableEntity();
            }*/
            return null;
        }

        [HttpPost("get")]
        public IActionResult GetUser([FromBody]KeyDto keyData)
        {
            /*var response = _communicationService.Send<KeyDto, UserDto>("api/user/get", keyData).Result;
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    return Ok(response.Value);
                default:
                    return UnprocessableEntity();
            }*/
            return null;
        }

    }
}