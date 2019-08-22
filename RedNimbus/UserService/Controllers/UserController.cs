using Microsoft.AspNetCore.Mvc;
using RedNimbus.UserService.Model;
using RedNimbus.UserService.Services.Interfaces;
using RedNimbus.DTO;
using RedNimbus.Either.Mappings;
using RedNimbus.Either;
using Either;
using RedNimbus.Either.Errors;

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


        private IActionResult UnprocessableEntityErr(IError error)
        {
            return UnprocessableEntity(error);
        }

        [HttpPost]
        public IActionResult Post([FromBody]CreateUserDto createUserDto)
        {
            var a = _mapper.Map<User>(createUserDto);
            var b = a.Map(_userService.Create);
            var c = b.Map(AllOk);
            var d = c.Reduce(UnprocessableEntityErr);
            return d; 
                
                
                
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthorizeUserDto userLoginDTO)
        {
            /* var userModel = _mapper.Map<User>(userLoginDTO)
             var user = _userService.Authenticate(userModel);

             if (user == null)
                 return UnprocessableEntity(new { message = "Username or password incorrect." });

             UserDto userData = _mapper.Map<UserDto>(user);
             userData.Key = _tokenService.GenerateToken();
             _userService.AddAuthenticatedUser(userData.Key, userData.Email);
             KeyDto keyData = _mapper.Map<KeyDto>(userData);
             return Ok(keyData);
             */
            return null;
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