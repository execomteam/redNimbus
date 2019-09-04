using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AutoMapper;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.IdentityModel.Tokens;
using NetMQ;
using RedNimbus.Communication;
using RedNimbus.Domain;
using RedNimbus.Either.Mappings;
using RedNimbus.Messages;
using RedNimbus.UserService.Helper;
using UserService.Database;
using ErrorCode = RedNimbus.Either.Enums.ErrorCode;

namespace RedNimbus.UserService
{
    public class UserService : BaseService
    {
        private static readonly Dictionary<string, string> tokenEmailPairs = new Dictionary<string, string>();
        private IUserRepository _userRepository;
       
        public UserService(IUserRepository repository) : base()
        {
            Subscribe("RegisterUser", HandleRegisterUser);
            Subscribe("AuthenticateUser", HandleAuthenticateUser);
            Subscribe("GetUser", HandleGetUser);
            _userRepository = repository;
        }



        private bool Validate(Message<UserMessage> userMessage)
        {
            if (!Validation.IsFirstNameValid(userMessage.Data.FirstName))
            {
                SendErrorMessage("First Name is empty.", ErrorCode.FirstNameNullEmptyOrWhiteSpace, userMessage.Id);
                return false;
            }

            if (!Validation.IsLastNameValid(userMessage.Data.LastName))
            {
                SendErrorMessage("Last Name is empty.", ErrorCode.LastNameNullEmptyOrWhiteSpace, userMessage.Id);
                return false;
            }

            if (!Validation.IsEmailValid(userMessage.Data.Email))
            {
                SendErrorMessage("Invalid email format.", ErrorCode.EmailWrongFormat, userMessage.Id);
                return false;
            }

            if (!Validation.IsPasswordValid(userMessage.Data.Password))
            {
                SendErrorMessage("Password does not satisfy requirements.", ErrorCode.PasswordWrongFormat, userMessage.Id);
                return false;
            }

            return true;
        }

        private void HandleRegisterUser(NetMQMessage message)
        {
            Message<UserMessage> userMessage = new Message<UserMessage>(message);

            if(!Validate(userMessage))
            {
                return;
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
                //registeredUsers.Add(user.Email, user);
                this._userRepository.SaveUser(user);

                userMessage.Topic = "Response";

                NetMQMessage msg = userMessage.ToNetMQMessage();
                SendMessage(msg);
            }
            catch (ArgumentException)
            {
                SendErrorMessage("Email already exists.", ErrorCode.EmailAlreadyUsed, userMessage.Id);
            }
            catch (Exception)
            {
                SendErrorMessage("Internal server error.", ErrorCode.InternalServerError, userMessage.Id);
            }
        }

        private void HandleAuthenticateUser(NetMQMessage message)
        {
            Message<UserMessage> userMessage = new Message<UserMessage>(message);

            if (!Validation.IsEmailValid(userMessage.Data.Email))
            {
                SendErrorMessage("Invalid email format.", ErrorCode.EmailWrongFormat, userMessage.Id);
            }

            if (!Validation.IsPasswordValid(userMessage.Data.Password))
            {
                SendErrorMessage("Password does not satisfy requirements.", ErrorCode.PasswordWrongFormat, userMessage.Id);
            }

            var email = userMessage.Data.Email;
            //if (registeredUsers.ContainsKey(userMessage.Data.Email))
            if (_userRepository.CheckIfExists(email))
                {
                //var registeredUser = registeredUsers[userMessage.Data.Email];
                var registeredUser = _userRepository.GetUserByEmail(userMessage.Data.Email);
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

            SendErrorMessage("Invalid credentials.", ErrorCode.IncorrectEmailOrPassword, userMessage.Id);
        }

        private void HandleGetUser(NetMQMessage message)
        {
            Message<TokenMessage> tokenMessage = new Message<TokenMessage>(message);

            if (tokenMessage.Data.Token == null || !tokenEmailPairs.ContainsKey(tokenMessage.Data.Token))
            {
                SendErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
            }

            if (tokenEmailPairs.ContainsKey(tokenMessage.Data.Token))
            {
                string email = tokenEmailPairs[tokenMessage.Data.Token];
                if (!_userRepository.CheckIfExists(email))
                {
                    SendErrorMessage("Requested user data not found", ErrorCode.UserNotRegistrated, tokenMessage.Id);
                }

                User registeredUser = _userRepository.GetUserByEmail(email);
                registeredUser.Key = tokenMessage.Data.Token;

                Message<UserMessage> userMessage = new Message<UserMessage>("Response");

                userMessage.Id = tokenMessage.Id;
                userMessage.Data.FirstName = registeredUser.FirstName;
                userMessage.Data.LastName = registeredUser.LastName;

                NetMQMessage msg = userMessage.ToNetMQMessage();
                SendMessage(msg);
            }
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
    }
}