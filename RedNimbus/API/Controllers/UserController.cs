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
        private ILogSender _logSender;

        public UserController(IUserService userService, IEitherMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
            _logSender = new LogSender("tcp://127.0.0.1:8887");
        }

        [HttpPost]
        public IActionResult Post([FromBody]CreateUserDto createUserDto)
        {
            Guid requestId = Guid.NewGuid();

            return _mapper.Map<User>(createUserDto)
                .Map((u) => SendLogMessage(requestId, u, "UserController/Post - Request Received"))
                .Map(_userService.RegisterUser)
                .Map((u) => SendLogMessage(requestId, u, "UserController/Post - Successful"))
                .Map(() => AllOk())
                .Reduce(this.BadRequestErrorHandler, EmailAlreadyUsed)
                .Reduce(this.InternalServisErrorHandler);
        }

        private User SendLogMessage(Guid id, User u, string origin)
        {
            _logSender.Send(id, new LogMessage()
            {
                Date = DateTime.Now.ToShortDateString().ToString(),
                Time = DateTime.Now.TimeOfDay.ToString(),
                Type = LogMessage.Types.LogType.Info,
                Payload = u.ToString(),
                Sender = origin
            });

            return u;
        }

        private static bool EmailAlreadyUsed(IError err)
        {
            return err is FormatError formatError && formatError.Code == RedNimbus.Either.Enums.ErrorCode.EmailAlreadyUsed;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateUserDto authenticateUserDto)
        {
            Guid requestId = Guid.NewGuid();

            return _mapper.Map<User>(authenticateUserDto)
               .Map((u) => SendLogMessage(requestId, u, "UserController/Authenticate - Request Received"))
               .Map(_userService.Authenticate)
               .Map((u) => SendLogMessage(requestId, u, "UserController/Authenticate - Successful"))
               .Map(x => AllOk(new KeyDto() { Key = x.Key }))
               .Reduce(AuthenticationErrorHandler, err => err is AuthenticationError)
               .Reduce(InternalServisErrorHandler);
        }

        [HttpGet]
        public IActionResult Get()
        {
            return _userService.GetUserByToken(Request.Headers["token"])
                .Map(_mapper.Map<UserDto>)
                .Map(x => AllOk(x))
                .Reduce(NotFoundErrorHandler, err => err is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }
    }
}