using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using RedNimbus.API.DTO;
using RedNimbus.API.Model;
using RedNimbus.API.Services;

namespace RedNimbus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class AccountController : ControllerBase
    {
        private IUserService _userService;

        public AccountController()
        {
            _userService = new UserService();
        }

        [HttpPost("[action]")]      
        public HttpResponseMessage Register(User user)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            if(_userService.Create(user) == null)
            {
                response.StatusCode = HttpStatusCode.UnprocessableEntity;
            }
            else
            {
                response.StatusCode = HttpStatusCode.Created;
            }

            return response;
        }

        [HttpPost("[action]")]
        public HttpResponseMessage Login(UserLoginDTO userDTO)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            if(_userService.Login(userDTO) == null)
            {
                response.StatusCode = HttpStatusCode.UnprocessableEntity;
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
            }

            return response;
        }
    }
}