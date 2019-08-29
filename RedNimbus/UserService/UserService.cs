using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NetMQ;
using RedNimbus.Communication;
using RedNimbus.Domain;
using RedNimbus.Messages;
using RedNimbus.UserService.Helper;

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

        #region Validation functions
        private bool IsFirstNameValid(string firstName)
        {
            return Regex.IsMatch(firstName, @"^[a-z A-Z]+$");
        }

        private bool IsLastNameValid(string lastName)
        {
            return Regex.IsMatch(lastName, @"^[a-z A-Z]+$");
        }

        private bool IsEmailValid(string email)
        {
            return RegexUtilities.IsValidEmail(email);
        }

        private bool IsPasswordValid(string password)
        {
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,24}$") && !String.IsNullOrWhiteSpace(password);
        }

        private bool IsPhoneValid(string phoneNumber)
        {
            if (!String.IsNullOrWhiteSpace(phoneNumber))
                return Regex.IsMatch(phoneNumber, @"^[0-9()-]+$");

            return true;
        }

        # endregion

        private void HandleRegisterUser(NetMQMessage message)
        {
            Message<UserMessage> userMessage = new Message<UserMessage>(message);

            if (!IsFirstNameValid(userMessage.Data.FirstName))
            {
                SendErrorMessage("First Name is empty.", RedNimbus.Either.Enums.ErrorCode.FirstNameNullEmptyOrWhiteSpace, userMessage.Id);
            }

            if (!IsLastNameValid(userMessage.Data.LastName))
            {
                SendErrorMessage("Last Name is empty.", RedNimbus.Either.Enums.ErrorCode.LastNameNullEmptyOrWhiteSpace, userMessage.Id);
            }

            if (!IsEmailValid(userMessage.Data.Email))
            {
                SendErrorMessage("Invalid email format.", RedNimbus.Either.Enums.ErrorCode.EmailWrongFormat, userMessage.Id);
            }

            if (!IsPasswordValid(userMessage.Data.Password))
            {
                SendErrorMessage("Password does not satisfy requirements.", RedNimbus.Either.Enums.ErrorCode.PasswordWrongFormat, userMessage.Id);
            }

            User user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = userMessage.Data.FirstName,
                LastName = userMessage.Data.LastName,
                Email = userMessage.Data.Email,
                Password = HashHelper.ComputeHash(userMessage.Data.Password),
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
                SendErrorMessage("Email already exists.", RedNimbus.Either.Enums.ErrorCode.EmailAlreadyUsed, userMessage.Id);
            }
            catch (Exception)
            {
                SendErrorMessage("Internal server error.", RedNimbus.Either.Enums.ErrorCode.InternalServerError, userMessage.Id);
            }
        }

        private void HandleAuthenticateUser(NetMQMessage message)
        {
            // TODO: Implement authentication

            throw new NotImplementedException();
        }

        private void SendErrorMessage(string messageText, RedNimbus.Either.Enums.ErrorCode errorCode, NetMQFrame idFrame)
        {
            Message<ErrorMessage> errorMessage = new Message<ErrorMessage>("Error");

            errorMessage.Data.MessageText = messageText;
            errorMessage.Data.ErrorCode = (int) errorCode;
            errorMessage.Id = idFrame;

            NetMQMessage msg = errorMessage.ToNetMQMessage();
            SendMessage(msg);
        }
    }
}