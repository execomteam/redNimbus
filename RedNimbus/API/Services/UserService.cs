using RedNimbus.API.Services.Interfaces;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RedNimbus.Messages;
using RedNimbus.Communication;
using RedNimbus.DTO;
using NetMQ;

namespace RedNimbus.API.Services
{
    public class UserService : IUserService
    {
        public Either<IError, TSuccess> RegisterUser<TRequest, TSuccess>(CreateUserDto createUserDto)
        {
            Message<UserMessage> message = new Message<UserMessage>("RegisterUser");

            message.Data.FirstName = createUserDto.FirstName;
            message.Data.LastName = createUserDto.LastName;
            message.Data.Email = createUserDto.Email;
            message.Data.Password = createUserDto.Password;
            message.Data.PhoneNumber = createUserDto.PhoneNumber;

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage());

            Either<IError, TSuccess> result = null;

            // TODO: Convert response to result (type of Either)

            return result;
        }
    }
}
