using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RedNimbus.UserService.Model;
using RedNimbus.UserService.Services.Interfaces;
using RedNimbus.UserService.Helper;
using RedNimbus.DTO;

namespace RedNimbus.UserService.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public UserController(IMapper mapper, IUserService userService, ITokenService tokenService)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost]
        public IActionResult Post([FromBody]CreateUserDto createUserDto)
        {
            var user = _mapper.Map<User>(createUserDto);
            var result = _userService.Create(user);

            if (result == null)
                return UnprocessableEntity(new { message = "Invalid user data." });

            return Ok();
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthorizeUserDto userLoginDTO)
        {
            var userModel = _mapper.Map<User>(userLoginDTO);
            var user = _userService.Authenticate(userModel);

            if (user == null)
                return UnprocessableEntity(new { message = "Username or password incorrect." });

            UserDto userData = _mapper.Map<UserDto>(user);
            userData.Key = _tokenService.GenerateToken();
            _userService.AddAuthenticatedUser(userData.Key, userData.Email);
            KeyDto keyData = _mapper.Map<KeyDto>(userData);
            return Ok(keyData);
        }

        [HttpPost("get")]
        public IActionResult GetUser([FromBody]KeyDto keyData)
        {
            User user = _userService.GetUserByToken(keyData.Key);
            UserDto userData = _mapper.Map<UserDto>(user);
            if(userData==null)
                return UnprocessableEntity(new { message = "Token not found." });
            userData.Key = keyData.Key;
            return Ok(userData);
        }

    }
}