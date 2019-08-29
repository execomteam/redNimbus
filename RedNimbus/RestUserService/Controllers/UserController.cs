using Microsoft.AspNetCore.Mvc;
using RedNimbus.Domain;
using RedNimbus.RestUserService.Services.Interfaces;
using RedNimbus.DTO;
using RedNimbus.Either.Mappings;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace RedNimbus.RestUserService.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IEitherMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public UserController(IEitherMapper mapper, IUserService userService, ITokenService tokenService)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _userService = userService;
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
            return NotFound(error);
        }

        private IActionResult AuthenticationErrorHandler(IError error)
        {
            return StatusCode(StatusCodes.Status406NotAcceptable, error);
        }

        private UserDto InsertToken(UserDto u)
        {
            u.Key = _tokenService.GenerateToken();
            return u;
        }

        [HttpPost]
        public IActionResult Post([FromBody]CreateUserDto createUserDto)
        {
            return _mapper.Map<User>(createUserDto)
                .Map(_userService.Create)
                .Map(()=>AllOk())
                .Reduce(BadRequestErrorHandler, err => err is FormatError)
                .Reduce(InternalServisErrorHandler); //ako bude greska vrati u lambdu
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateUserDto userLoginDTO)
        {
            return _mapper.Map<User>(userLoginDTO) //form AuthUserDto to User
               .Map(_userService.Authenticate)
               .Map(_mapper.Map<UserDto>)
               .Map(InsertToken)
               .Map(_userService.AddAuthenticatedUser)
               .Map(x => AllOk(new KeyDto() { Key = x.Key }))
               .Reduce(AuthenticationErrorHandler, err => err is AuthenticationError)
               .Reduce(InternalServisErrorHandler); 

             //KeyDto keyData = _mapper.Map<KeyDto>(userData);
             //return Ok(keyData);
            
        }

        //greskaaaaa get ne post

        [HttpPost("get")]
        public IActionResult GetUser([FromBody]KeyDto keyData)
        {
            return _userService.GetUserByToken(keyData.Key)
                .Map(_mapper.Map<UserDto>)
                .Map((either) => AllOk(either))
                .Reduce(NotFoundErrorHandler, err => err is NotFoundError)
                .Reduce(InternalServisErrorHandler);           
        }
    }
}