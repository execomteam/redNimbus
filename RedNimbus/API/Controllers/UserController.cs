using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedNimbus.API.Services.Interfaces;
using RedNimbus.Domain;
using RedNimbus.DTO;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using RedNimbus.Either.Mappings;

namespace RedNimbus.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEitherMapper _mapper;

        public UserController(IUserService userService, IEitherMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

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
            return BadRequest(error);
        }

        private IActionResult InternalServisErrorHandler(IError error)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, error);
        }
        private IActionResult NotFoundErrorHandler(IError error)
        {
            return NotFound(error.Message);
        }

        private IActionResult AuthenticationErrorHandler(IError error)
        {
            return StatusCode(StatusCodes.Status406NotAcceptable, error);
        }
        private static bool EmailAlreadyUsed(IError err)
        {
            return err is FormatError formatError && formatError.Code == RedNimbus.Either.Enums.ErrorCode.EmailAlreadyUsed;
        }

        [HttpPost]
        public IActionResult Post([FromBody]CreateUserDto createUserDto)
        {

            return _mapper.Map<User>(createUserDto)
                .Map(_userService.RegisterUser)
                .Map(() => AllOk())
                .Reduce(this.BadRequestErrorHandler, EmailAlreadyUsed)
                .Reduce(this.InternalServisErrorHandler);
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateUserDto authenticateUserDto) => _mapper.Map<User>(authenticateUserDto)
               .Map(_userService.Authenticate)
               .Map(x => AllOk(new KeyDto() { Key = x.Key }))
               .Reduce(AuthenticationErrorHandler, err => err is AuthenticationError)
               .Reduce(InternalServisErrorHandler);
         
        [HttpGet]
        public IActionResult Get()
        {
            return _userService.GetUserByToken(Request.Headers["token"])
                .Map(_mapper.Map<UserDto>)
                .Map(x => AllOk(x))
                .Reduce(NotFoundErrorHandler, err => err is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }
    }
}