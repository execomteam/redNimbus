using RedNimbus.API.Services.Interfaces;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using RedNimbus.Messages;
using RedNimbus.Communication;
using RedNimbus.DTO;
using RedNimbus.Domain;
using NetMQ;

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



            // TODO: Convert response to result (type of Either)

            if (responseTopic.Equals("Response"))
            {
                Message<UserMessage> successMessage = new Message<UserMessage>(response);

                // TODO: Ne vracati user objekat

                return new Right<IError, User>(user);
            }

            Message<ErrorMessage> errorMessage = new Message<ErrorMessage>(response);
            return new Left<IError, User>(new FormatError(errorMessage.Data.Message, Either.Enums.ErrorCode.EmailAlreadyUsed));
            
        }

        public Either<IError, TSuccess> AuthenticateUser<TRequest, TSuccess>(AuthenticateUserDto authenticateUserDto)
        {
            throw new System.NotImplementedException();
        }
    }
}
