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
    [Route("api/v2/user")]
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
        public IActionResult Post([FromBody]CreateUserDto createUserDto) => _mapper.Map<User>(createUserDto)
                .Map(_userService.RegisterUser)
                .Map(() => AllOk())
                .Reduce(BadRequestErrorHandler, err => err is FormatError)
                .Reduce(InternalServisErrorHandler);

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateUserDto userLoginDTO)
        {
            //return _mapper.Map<User>(userLoginDTO)
            //   .Map(_userService.Authenticate)
            //   .Map(_mapper.Map<UserDto>)
            //   .Map(InsertToken)
            //   .Map(_userService.AddAuthenticatedUser)
            //   .Map(x => AllOk(new KeyDto() { Key = x.Key }))
            //   .Reduce(AuthenticationErrorHandler, err => err is AuthenticationError)
            //   .Reduce(InternalServisErrorHandler);

            return null;
        }

        // TODO: GET
    }
}