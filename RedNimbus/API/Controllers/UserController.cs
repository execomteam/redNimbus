using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RedNimbus.API.DTO;
using RedNimbus.API.Model;
using RedNimbus.API.Services;

namespace RedNimbus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IOptions<JwtConfiguration> config)
        {
            _userService = new UserService(config);
        }

        [HttpPost("[action]")]      
        public IActionResult Register(User user)
        {
            User result = _userService.Create(user);

            if(result == null)
            {
                return UnprocessableEntity();
            }

            return Ok();
        }

        [HttpPost("[action]")]
        public IActionResult Login(UserLoginDTO userLoginDTO)
        {
            UserDTO result = _userService.Login(userLoginDTO);

            if (result == null)
            {
                return UnprocessableEntity();
            }
            return Ok(result);
        }
    }
}