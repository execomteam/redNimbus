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
    public class UserService : IUserService
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

        private IError GetError(NetMQMessage response)
        {
            Message<ErrorMessage> errorMessage = new Message<ErrorMessage>(response);

            ErrorCode errorCode = (ErrorCode)Enum.ToObject(typeof(ErrorCode), errorMessage.Data.ErrorCode);

            switch (errorCode)
            {
                case ErrorCode.FirstNameNullEmptyOrWhiteSpace:
                case ErrorCode.LastNameNullEmptyOrWhiteSpace:
                case ErrorCode.EmailWrongFormat:
                case ErrorCode.PasswordWrongFormat:
                case ErrorCode.EmailAlreadyUsed:
                case ErrorCode.PasswordsDoNotMatch:
                    return new FormatError(errorMessage.Data.MessageText, errorCode);
                case ErrorCode.IncorrectEmailOrPassword:
                    return new AuthenticationError(errorMessage.Data.MessageText, errorCode);
                case ErrorCode.UserNotFound:
                case ErrorCode.UserNotRegistrated:
                    return new NotFoundError(errorMessage.Data.MessageText, errorCode);
                default:
                    return new InternalServisError(errorMessage.Data.MessageText, errorCode);
            }
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
                    LastName = successMessage.Data.LastName,
                    Key = successMessage.Data.Token
                };

                return new Right<IError, User>(user);
            }

            return new Left<IError, User>(GetError(response));
        }

    }
}
