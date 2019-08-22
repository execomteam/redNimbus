using Microsoft.AspNetCore.Mvc;
using RedNimbus.UserService.Model;
using RedNimbus.UserService.Services.Interfaces;
using RedNimbus.DTO;
using RedNimbus.Either.Mappings;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using System.Net;

namespace RedNimbus.UserService.Controllers
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


        private IActionResult UnprocessableEntityErr(IError error)
        {
            return UnprocessableEntity(error);
        }

        private IActionResult InternalServisErr(IError error)
        {
            return BadRequest(error);
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
                .Reduce(UnprocessableEntityErr, err => err is UnacceptableFormatErr)
                .Reduce(x=>InternalServisErr(x));
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthorizeUserDto userLoginDTO)
        {
               return _mapper.Map<UserDto>(
                    _mapper.Map<User>(userLoginDTO) //form AuthUserDto to User
                    .Map(_userService.Authenticate) //auth
               )
               .Map(InsertToken)
               .Map(_userService.AddAuthenticatedUser)
               .Map(x => AllOk(x.Key))
               .Reduce(UnprocessableEntityErr, err => err is UnacceptableFormatErr)
               .Reduce(x => InternalServisErr(x));

             //KeyDto keyData = _mapper.Map<KeyDto>(userData);
             //return Ok(keyData);
            
        }

        [HttpPost("get")]
        public IActionResult GetUser([FromBody]KeyDto keyData)
        {
            /*
            User user = _userService.GetUserByToken(keyData.Key);
            UserDto userData = _mapper.Map<UserDto>(user);
            if(userData==null)
                return UnprocessableEntity(new { message = "Token not found." });
            userData.Key = keyData.Key;
            return Ok(userData);
            */
            return null;
        }

    }
}