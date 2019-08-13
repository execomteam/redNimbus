using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RedNimbus.UserService.Model;
using RedNimbus.UserService.Services;
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
        private readonly JwtConfiguration _jwtConfiguration;
        private IUserService _userService;

        public UserController(IMapper mapper, IOptions<JwtConfiguration> jwtConfiguration)
        {
            _mapper = mapper;
            _jwtConfiguration = jwtConfiguration.Value;
            _userService = new UserService.Services.UserService();
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
            userData.Key = GenerateJwt();

            return Ok(userData);
        }

        private string GenerateJwt()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(_jwtConfiguration.Issuer,
              _jwtConfiguration.Issuer,
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}