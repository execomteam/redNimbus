using RedNimbus.API.Services.Interfaces;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using RedNimbus.Messages;
using RedNimbus.Communication;
using RedNimbus.DTO;
using RedNimbus.Domain;
using NetMQ;
using System;

namespace RedNimbus.API.Services
{
    public class UserService : IUserService
    {
        public UserService()
        {

        }

        public Either<IError, User> RegisterUser(User user)
        {
            Message<UserMessage> message = new Message<UserMessage>("RegisterUser");

            message.Data.FirstName = user.FirstName;
            message.Data.LastName = user.LastName;
            message.Data.Email = user.Email;
            message.Data.Password = user.Password;
            //message.Data.PhoneNumber = user.PhoneNumber;

            // TODO: Fix message constructor

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

            // TODO: Fix message constructor

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

            RedNimbus.Either.Enums.ErrorCode errorCode = (RedNimbus.Either.Enums.ErrorCode)Enum.ToObject(typeof(RedNimbus.Either.Enums.ErrorCode), errorMessage.Data.ErrorCode);

            switch (errorCode)
            {
                case RedNimbus.Either.Enums.ErrorCode.FirstNameNullEmptyOrWhiteSpace:
                case RedNimbus.Either.Enums.ErrorCode.LastNameNullEmptyOrWhiteSpace:
                case RedNimbus.Either.Enums.ErrorCode.EmailWrongFormat:
                case RedNimbus.Either.Enums.ErrorCode.PasswordWrongFormat:
                case RedNimbus.Either.Enums.ErrorCode.EmailAlreadyUsed:
                    return new FormatError(errorMessage.Data.MessageText, errorCode);
                case RedNimbus.Either.Enums.ErrorCode.UserNotFound:
                case RedNimbus.Either.Enums.ErrorCode.UserNotRegistrated:
                    return new NotFoundError(errorMessage.Data.MessageText, errorCode);
                default:
                    return new InternalServisError(errorMessage.Data.MessageText, errorCode);
            }
        }

        public Either<IError, User> GetUserByToken(string token)
        {
            Message<TokenMessage> message = new Message<TokenMessage>("GetUser");

            message.Data.Token = token;

            // TODO: Fix message constructor

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
