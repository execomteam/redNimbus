using System;
using System.Collections.Generic;
using NetMQ;
using RedNimbus.Communication;
using RedNimbus.Domain;
using RedNimbus.Messages;
using RedNimbus.RestUserService.Helper;

namespace UserService
{
    public class UserService : BaseService
    {
        private static readonly Dictionary<string, User> registeredUsers = new Dictionary<string, User>();

        public UserService() : base()
        {
            Subscribe("RegisterUser", HandleRegisterUser);
            Subscribe("AuthenticateUser", HandleAuthenticateUser);
        }

        private void HandleRegisterUser(NetMQMessage message)
        {
            Message<UserMessage> userMessage = new Message<UserMessage>(message);

            // TODO: Validate fields

            User user = new User
            {
                Id          = Guid.NewGuid(),
                FirstName   = userMessage.Data.FirstName,
                LastName    = userMessage.Data.LastName,
                Email       = userMessage.Data.Email,
                Password    = HashHelper.ComputeHash(userMessage.Data.Password),
                PhoneNumber = userMessage.Data.PhoneNumber,
            };

            try
            {
                registeredUsers.Add(user.Email, user);

                userMessage.Topic = "Response";

                NetMQMessage msg = userMessage.ToNetMQMessage();
                SendMessage(msg);
            }
            catch (ArgumentException)
            {
                Message<ErrorMessage> errorMessage = new Message<ErrorMessage>("Error");

                errorMessage.Id = userMessage.Id;
                errorMessage.Data.Message = "Email already exists";

                NetMQMessage msg = errorMessage.ToNetMQMessage();
                SendMessage(msg);
            }
            catch (Exception)
            {
                Message<ErrorMessage> errorMessage = new Message<ErrorMessage>("Error");

                errorMessage.Id = userMessage.Id;
                errorMessage.Data.Message = "Internal server error.";

                NetMQMessage msg = errorMessage.ToNetMQMessage();
                SendMessage(msg);
            }
        }

        private void HandleAuthenticateUser(NetMQMessage message)
        {
            // TODO: Implement authentication

            throw new NotImplementedException();
        }
    }
}