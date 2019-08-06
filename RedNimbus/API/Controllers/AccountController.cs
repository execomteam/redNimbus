using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedNimbus.API.DTO;
using RedNimbus.API.Model;

namespace RedNimbus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class AccountController : ControllerBase
    {
        public static Dictionary<string, User> registratedUsers = new Dictionary<string, User>();

        [HttpPost("[action]")]      
        public HttpResponseMessage Register(User user)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            if (!ModelState.IsValid)
            {
                response.StatusCode = HttpStatusCode.UnprocessableEntity;
                return response;
            }

            if(registratedUsers.ContainsKey(user.Email))
            {
                response.StatusCode = HttpStatusCode.Conflict;
                return response;
            }

            registratedUsers.Add(user.Email, user);
            response.StatusCode = HttpStatusCode.Created;

            return response;
        }

        [HttpPost("login")]
        public HttpResponseMessage Login(UserLoginDTO userDTO)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            if (!ModelState.IsValid)
            {
                response.StatusCode = HttpStatusCode.UnprocessableEntity;
                return response;
            }

            if (registratedUsers.ContainsKey(userDTO.Email))
            {
                var tempUser = registratedUsers[userDTO.Email];
                if(tempUser.Password == userDTO.Password)
                {
                    response.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.UnprocessableEntity;
                }
            }
            else
            {
                response.StatusCode = HttpStatusCode.UnprocessableEntity;
            }

            return response;
        }
    }
}