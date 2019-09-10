using RedNimbus.API.Services.Interfaces;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using RedNimbus.Messages;
using RedNimbus.Communication;
using RedNimbus.DTO;
using RedNimbus.Domain;
using NetMQ;
using RedNimbus.API.Helper;

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

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage());

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
            Message<UserMessage> message = new Message<UserMessage>("AuthenticateUser")
            {
                Data = new UserMessage()
                {
                    Email = user.Email,
                    Password = user.Password
                }
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage());

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
            Message<TokenMessage> message = new Message<TokenMessage>("GetUser")
            {
                Data = new TokenMessage()
                {
                    Token = token
                }
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage());

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
