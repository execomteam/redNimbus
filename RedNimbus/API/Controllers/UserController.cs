﻿
using Microsoft.AspNetCore.Mvc;
using RedNimbus.API.Models;
using RedNimbus.API.Services;
using RedNimbus.API.Services.Interfaces;
using RedNimbus.DTO;

namespace RedNimbus.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly ICommunicationService _communicationService;

        public UserController(ICommunicationService communicationService)
        {
            _communicationService = communicationService;
        }

        [HttpPost]
        public IActionResult Post([FromBody]CreateUserDto createUserDto)
        {
            var response = _communicationService.Send<CreateUserDto, EmptyResponse>("api/user", createUserDto).Result;

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    return Ok();
                default:
                    return UnprocessableEntity();
            }
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthorizeUserDto userLoginDTO)
        {
            var response = _communicationService.Send<AuthorizeUserDto, DtoResponse>("api/user/authenticate", userLoginDTO).Result; 
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    return Ok(response.Value);
                default:
                    return UnprocessableEntity();
            }

        }
    }
}