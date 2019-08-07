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
using System.Security.Cryptography; 
using System.Text;

namespace RedNimbus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class AccountController : ControllerBase
    {
        public static Dictionary<string, User> registratedUsers = new Dictionary<string, User>();

        static string ComputeSha256Hash(string rawData)  
        {  
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())  
            {  
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));  
  
                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();  
                for (int i = 0; i < bytes.Length; i++)  
                {  
                    builder.Append(bytes[i].ToString("x2"));  
                }  
                return builder.ToString();  
            }  
        }

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

            user.Password = ComputeSha256Hash(user.Password);
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
                if(tempUser.Password == ComputeSha256Hash(userDTO.Password))
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