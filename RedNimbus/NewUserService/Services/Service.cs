using NetMQ;
using RedNimbus.Communication;
using RedNimbus.Messages;
using RedNimbus.NewUserService.Helper;
using RedNimbus.UserService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.NewUserService.Services
{
    public class Service : BaseService
    {
        private static readonly Dictionary<string, User> registeredUsers = new Dictionary<string, User>();

        public Service() : base()
        {
            Subscribe("RegisterUser", CreateUser);
        }

        private void CreateUser(NetMQMessage message)
        {
            Message<UserMessage> userMessage = new Message<UserMessage>(message);

            //TODO: Srediti referencu na model - dto
            User user = new User();

            user.Id = Guid.NewGuid();
            user.Password = HashHelper.ComputeHash(userMessage.Data.Password);
            user.FirstName = userMessage.Data.FirstName;
            user.LastName = userMessage.Data.LastName;
            user.PhoneNumber = userMessage.Data.PhoneNumber;
            user.Email = userMessage.Data.Email;

            registeredUsers.Add(user.Email, user);

            userMessage.Topic = "Response";

            NetMQMessage msg = userMessage.ToNetMQMessage();
            SendMessage(msg);

        }
    }
}
