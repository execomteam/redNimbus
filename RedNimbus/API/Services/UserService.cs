using RedNimbus.API.Services.Interfaces;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using RedNimbus.Messages;
using RedNimbus.Communication;
using RedNimbus.DTO;
using RedNimbus.Domain;
using NetMQ;
using System;
using RedNimbus.API.Helper;
using ErrorCode = RedNimbus.Either.Enums.ErrorCode;

namespace RedNimbus.API.Services
{
    public class UserService : BaseService, IUserService
    {
        public Either<IError, User> RegisterUser(User user)
        {
            Message<UserMessage> message = new Message<UserMessage>("RegisterUser")
            {
                Data = new UserMessage()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = user.Password
                }
            };
           
            NetMQMessage temp = message.ToNetMQMessage();
            NetMQFrame topicFrame = temp.Pop();
            NetMQFrame emptyFrame = temp.Pop();
            temp.Push(topicFrame);
        
            NetMQMessage response = RequestSocketFactory.SendRequest(temp);

            string responseTopic = response.First.ConvertToString();


            if (responseTopic.Equals("Response"))
            {
                Message<UserMessage> successMessage = new Message<UserMessage>(response);

                return new Right<IError, User>(user);
            }

            return new Left<IError, User>(GetError(response));
        }

        public Either<IError, KeyDto> Authenticate(User user)
        {
            Message<UserMessage> message = new Message<UserMessage>("AuthenticateUser");

            message.Data.Email = user.Email;
            message.Data.Password = user.Password;

            NetMQMessage temp = message.ToNetMQMessage();
            NetMQFrame topicFrame = temp.Pop();
            NetMQFrame emptyFrame = temp.Pop();
            temp.Push(topicFrame);

            NetMQMessage response = RequestSocketFactory.SendRequest(temp);

            string responseTopic = response.First.ConvertToString();

            if (responseTopic.Equals("Response"))
            {
                Message<TokenMessage> successMessage = new Message<TokenMessage>(response);

                KeyDto keyDto = new KeyDto();
                keyDto.Key = successMessage.Data.Token;

                return new Right<IError, KeyDto>(keyDto);
            }

            return new Left<IError, KeyDto>(GetError(response));
        }

        

        public Either<IError, User> GetUserByToken(string token)
        {
            Message<TokenMessage> message = new Message<TokenMessage>("GetUser");

            message.Data.Token = token;

            NetMQMessage temp = message.ToNetMQMessage();
            NetMQFrame topicFrame = temp.Pop();
            NetMQFrame emptyFrame = temp.Pop();
            temp.Push(topicFrame);

            NetMQMessage response = RequestSocketFactory.SendRequest(temp);

            string responseTopic = response.First.ConvertToString();

            if (responseTopic.Equals("Response"))
            {
                Message<UserMessage> successMessage = new Message<UserMessage>(response);

                User user = new User
                {
                    FirstName = successMessage.Data.FirstName,
                    LastName = successMessage.Data.LastName
                };

                return new Right<IError, User>(user);
            }

            return new Left<IError, User>(GetError(response));
        }

    }
}
