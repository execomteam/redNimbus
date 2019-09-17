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

            return _mapper.Map<User>(createUserDto, (u) => LogRequest(requestId, u))
                .Map(_userService.RegisterUser)
                .Map(() => AllOk(), (u) => LogSuccessfulPost(requestId, u))
                .Reduce(this.BadRequestErrorHandler, EmailAlreadyUsed, (e) => LogError(requestId, e, "UserController/Post"))
                .Reduce(this.InternalServisErrorHandler, (e) => LogError(requestId, e, "UserController/Post"));
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateUserDto authenticateUserDto)
        {
            Guid requestId = Guid.NewGuid();

            return _mapper.Map<User>(authenticateUserDto, (u) => LogRequest(requestId, u))
               .Map(_userService.Authenticate)
               .Map(x => AllOk(new KeyDto() { Key = x.Key }), (x) => LogSuccessfulAuthentication(requestId, x))
               .Reduce(AuthenticationErrorHandler, err => err is AuthenticationError, (e) => LogError(requestId, e, "UserController/Authernticate"))
               .Reduce(InternalServisErrorHandler, (e) => LogError(requestId, e, "UserController/Authernticate"));
        }

        [HttpGet]
        public IActionResult Get()
        {
            Guid requestId = Guid.NewGuid();
            LogRequest(requestId);

            return _userService.GetUserByToken(Request.Headers["token"])
                .Map(_mapper.Map<UserDto>)
                .Map(x => AllOk(x), (u) => LogSuccessfulGet(requestId, u))
                .Reduce(NotFoundErrorHandler, err => err is NotFoundError, (e) => LogError(requestId, e, "UserController/Get"))
                .Reduce(InternalServisErrorHandler, (e) => LogError(requestId, e, "UserController/Get"));
        }

        private void LogSuccessfulPost(Guid id, User u)
        {
            _logSender.Send(id, new LogMessage()
            {
                Date = DateTime.Now.ToShortDateString().ToString(),
                Time = DateTime.Now.TimeOfDay.ToString(),
                Type = LogMessage.Types.LogType.Info,
                Payload = u.ToString(),
                Sender = "UserController/Post"
            });
        }

        private void LogSuccessfulGet(Guid id, UserDto userDto)
        {
            _logSender.Send(id, new LogMessage()
            {
                Date = DateTime.Now.ToShortDateString().ToString(),
                Time = DateTime.Now.TimeOfDay.ToString(),
                Type = LogMessage.Types.LogType.Info,
                Payload = userDto.ToString(),
                Sender = "UserController/Get"
            });
        }

        private void LogSuccessfulAuthentication(Guid id, KeyDto token)
        {
            _logSender.Send(id, new LogMessage()
            {
                Date = DateTime.Now.ToShortDateString().ToString(),
                Time = DateTime.Now.TimeOfDay.ToString(),
                Type = LogMessage.Types.LogType.Info,
                Payload = token.Key,
                Sender = "UserController/Authenticate"
            });
        }

        private void LogError(Guid id, IError e, string origin)
        {
            _logSender.Send(id, new LogMessage()
            {
                Date = DateTime.Now.ToShortDateString().ToString(),
                Time = DateTime.Now.TimeOfDay.ToString(),
                Type = LogMessage.Types.LogType.Error,
                Payload = e.Code.ToString(),
                Sender = origin
            });
        }

        private void LogRequest(Guid id, User u = null)
        {
            _logSender.Send(id, new LogMessage()
            {
                Date = DateTime.Now.ToShortDateString().ToString(),
                Time = DateTime.Now.TimeOfDay.ToString(),
                Type = LogMessage.Types.LogType.Info,
                Payload = u != null ? u.ToString() : "function does not take any parameters",
                Sender = "UserController/Post"
            });
        }

        private static bool EmailAlreadyUsed(IError err)
        {
            return err is FormatError formatError && formatError.Code == RedNimbus.Either.Enums.ErrorCode.EmailAlreadyUsed;
        }
    }
}