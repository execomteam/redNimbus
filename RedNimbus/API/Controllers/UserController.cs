using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedNimbus.API.Services.Interfaces;
using RedNimbus.Domain;
using RedNimbus.DTO;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using RedNimbus.Either.Mappings;
using RedNimbus.LogLibrary;
using RedNimbus.Messages;
using System;

namespace RedNimbus.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Produces("application/json")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IEitherMapper _mapper;
        private string _loginPage;

        public UserController(IUserService userService, IEitherMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
            _loginPage = "http://localhost:65001/login/";
        }

        [HttpPost]
        public IActionResult Post([FromBody]CreateUserDto createUserDto)
        {
            Guid requestId = Guid.NewGuid();
            
            return _mapper.Map<User>(createUserDto)
                .Map((x) => _userService.RegisterUser(x, requestId))
                .Map(() => AllOk())
                .Reduce(this.BadRequestErrorHandler, EmailAlreadyUsed)
                .Reduce(this.InternalServisErrorHandler);
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateUserDto authenticateUserDto)
        {
            Guid requestId = Guid.NewGuid();
           
            return _mapper.Map<User>(authenticateUserDto)
               .Map((u) => _userService.Authenticate(u, requestId))
               .Map(x => AllOk(new KeyDto() { Key = x.Key }))
               .Reduce(AuthenticationErrorHandler, err => err is AuthenticationError)
               .Reduce(InternalServisErrorHandler);
        }

        [HttpGet]
        public IActionResult Get()
        {
            Guid requestId = Guid.NewGuid();
            
            return _userService.GetUserByToken(Request.Headers["token"], requestId)
                .Map(_mapper.Map<UserDto>)
                .Map(x => AllOk(x))
                .Reduce(NotFoundErrorHandler, err => err is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }

        private static bool EmailAlreadyUsed(IError err)
        {
            return err is FormatError formatError && formatError.Code == RedNimbus.Either.Enums.ErrorCode.EmailAlreadyUsed;
        }

        [HttpPost("deactivateUserAccount")]
        public IActionResult deactivateAccount()
        {
            Guid requestId = Guid.NewGuid();
            var token = Request.Headers["token"];

            return _userService.deactivateUserAccount(Request.Headers["token"], requestId)
                .Map(x => AllOk(x))
                .Reduce(NotFoundErrorHandler, err => err is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }

        [HttpGet("emailConfirmation/{token}")]
        public IActionResult EmailConfirmation(string token)
        {
            Guid requestId = Guid.NewGuid();
           
            return _userService.EmailConfirmation(token, requestId)
                .Map(() => (IActionResult)Redirect(_loginPage))
                .Reduce(NotFoundErrorHandler, err => err is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }
    }
}