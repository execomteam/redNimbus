using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
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
        private static readonly Dictionary<string, string> tokenEmailPairs = new Dictionary<string, string>();

        public UserService() : base()
        {
            Subscribe("RegisterUser", HandleRegisterUser);
            Subscribe("AuthenticateUser", HandleAuthenticateUser);
            Subscribe("GetUser", HandleGetUser);
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
            Message<UserMessage> userMessage = new Message<UserMessage>(message);

            if (!IsEmailValid(userMessage.Data.Email))
            {
                SendErrorMessage("Invalid email format.", RedNimbus.Either.Enums.ErrorCode.EmailWrongFormat, userMessage.Id);
            }

            if (!IsPasswordValid(userMessage.Data.Password))
            {
                SendErrorMessage("Password does not satisfy requirements.", RedNimbus.Either.Enums.ErrorCode.PasswordWrongFormat, userMessage.Id);
            }

            if (registeredUsers.ContainsKey(userMessage.Data.Email))
            {
                var registeredUser = registeredUsers[userMessage.Data.Email];
                if (registeredUser.Password == HashHelper.ComputeHash(userMessage.Data.Password))
                {
                    Message<TokenMessage> tokenMessage = new Message<TokenMessage>("Response");

                    tokenMessage.Id = userMessage.Id;
                    tokenMessage.Data.Token = GenerateToken();

                    if (!tokenEmailPairs.ContainsKey(tokenMessage.Data.Token))
                    {
                        tokenEmailPairs.Add(tokenMessage.Data.Token, userMessage.Data.Email);
                    }

                    NetMQMessage msg = tokenMessage.ToNetMQMessage();
                    SendMessage(msg);
                }

            }

            SendErrorMessage("Invalid credentials.", RedNimbus.Either.Enums.ErrorCode.IncorrectEmailOrPassword, userMessage.Id);
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



        private string GenerateToken()
        {
            string key = "VerySecureSecretKey";
            string issuer = "RedNimbus";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(issuer,
              issuer,
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private void HandleGetUser(NetMQMessage message)
        {
            Message<TokenMessage> tokenMessage = new Message<TokenMessage>(message);

            if (tokenMessage.Data.Token == null || !tokenEmailPairs.ContainsKey(tokenMessage.Data.Token))
            {
                SendErrorMessage("Requested user data not found", RedNimbus.Either.Enums.ErrorCode.UserNotFound, tokenMessage.Id);
            }

            string email = tokenEmailPairs[tokenMessage.Data.Token];
            if (!registeredUsers.ContainsKey(email))
            {
                SendErrorMessage("Requested user data not found", RedNimbus.Either.Enums.ErrorCode.UserNotRegistrated, tokenMessage.Id);
            }

            User registeredUser = registeredUsers[email];
            registeredUser.Key = tokenMessage.Data.Token;

            Message<UserMessage> userMessage = new Message<UserMessage>("Response");

            userMessage.Id = tokenMessage.Id;
            userMessage.Data.FirstName = registeredUser.FirstName;
            userMessage.Data.LastName = registeredUser.LastName;

            NetMQMessage msg = userMessage.ToNetMQMessage();
            SendMessage(msg);
        }
    }
}