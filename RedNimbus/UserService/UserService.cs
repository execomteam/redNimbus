using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetMQ;
using RedNimbus.Communication;
using RedNimbus.Domain;
using RedNimbus.LogLibrary;
using RedNimbus.Messages;
using RedNimbus.TokenManager;
using RedNimbus.UserService.Helper;
using UserService.Database;
using ErrorCode = RedNimbus.Either.Enums.ErrorCode;

namespace RedNimbus.UserService
{
    public class UserService : BaseService
    {
        private static readonly Dictionary<string, string> tokenEmailPairs = new Dictionary<string, string>();
        private IUserRepository _userRepository;
        private ITokenManager _tokenManager;
        private ILogSender _logSender;

        public UserService(IUserRepository repository, ITokenManager tokenManager, ILogSender logSender) : base()
        {
            Subscribe("RegisterUser", HandleRegisterUser);
            Subscribe("AuthenticateUser", HandleAuthenticateUser);
            Subscribe("GetUser", HandleGetUser);
            
            _tokenManager = tokenManager;
            _userRepository = repository;
            _logSender = logSender;
        }



        private bool Validate(Message<UserMessage> userMessage)
        {
            if (!Validation.IsFirstNameValid(userMessage.Data.FirstName))
            {
                SendErrorMessage("Invalid First Name format.", ErrorCode.FirstNameNullEmptyOrWhiteSpace, userMessage.Id);
                return false;
            }

            if (!Validation.IsLastNameValid(userMessage.Data.LastName))
            {
                SendErrorMessage("Invalid Last Name format.", ErrorCode.LastNameNullEmptyOrWhiteSpace, userMessage.Id);
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
            if (!String.IsNullOrWhiteSpace(userMessage.Data.PhoneNumber) && !Validation.IsPhoneValid(userMessage.Data.PhoneNumber))
            {
                SendErrorMessage("Invalid phone number format.", ErrorCode.PhoneNumberWrongFormat, userMessage.Id);
                return false;
            }

            return true;
        }

        private void LogRequest<T>(NetMQMessage message, string origin) where T: IMessage, new()
        {
            T payload = new T();
            payload.MergeFrom(message[2].ToByteArray());

            LogMessage logMessage = new LogMessage()
            {
                Origin = origin,
                Payload = "**** " + payload.ToString(),
                Date = DateTime.Now.ToShortDateString(),
                Time = DateTime.Now.TimeOfDay.ToString(),
                Type = LogMessage.Types.LogType.Info
            };

            _logSender.Send(new Guid(message[1].ToByteArray()), logMessage);
        }

        private void HandleRegisterUser(NetMQMessage message)
        {
            LogRequest<UserMessage>(message, "UserService/HandleRegisterUser - Message received from Event bus");

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
                this._userRepository.SaveUser(user);

                userMessage.Topic = "Response";

                NetMQMessage msg = userMessage.ToNetMQMessage();
                SendMessage(msg);
            }
            catch (DbUpdateException)
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
            LogRequest<UserMessage>(message, "UserService/HandleAuthenticateUser - Message received from Event bus");

            Message<UserMessage> userMessage = new Message<UserMessage>(message);

            if (!Validation.IsEmailValid(userMessage.Data.Email))
            {
                SendErrorMessage("Email or password are not valid!", ErrorCode.IncorrectEmailOrPassword, userMessage.Id);
            }

            if (!Validation.IsPasswordValid(userMessage.Data.Password))
            {
                SendErrorMessage("Email or password are not valid!", ErrorCode.IncorrectEmailOrPassword, userMessage.Id);
            }

            var email = userMessage.Data.Email;
            if (_userRepository.CheckIfExists(email))
                {
                var registeredUser = _userRepository.GetUserByEmail(userMessage.Data.Email);
                if (registeredUser.Password == HashHelper.ComputeHash(userMessage.Data.Password))
                {
                    Message<TokenMessage> tokenMessage = new Message<TokenMessage>("Response");

                    tokenMessage.Id = userMessage.Id;
                    tokenMessage.Data.Token = _tokenManager.GenerateToken(registeredUser.Id);


                    if (!tokenEmailPairs.ContainsKey(tokenMessage.Data.Token))
                    {
                        tokenEmailPairs.Add(tokenMessage.Data.Token, userMessage.Data.Email);
                    }

                    NetMQMessage msg = tokenMessage.ToNetMQMessage();
                    SendMessage(msg);
                    return;
                }
            }

            SendErrorMessage("Email or password are not valid!", ErrorCode.IncorrectEmailOrPassword, userMessage.Id);
        }



        private void HandleGetUser(NetMQMessage message)
        {
            LogRequest<TokenMessage>(message, "UserService/HandleGetUser - Message received from Event bus");

            Message<TokenMessage> tokenMessage = new Message<TokenMessage>(message);

            if (tokenMessage.Data.Token == null)
            {
                SendErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
                return;
            }

            Guid id = _tokenManager.ValidateToken(tokenMessage.Data.Token);
            if (id.Equals(Guid.Empty))
            {
                SendErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
                return;
            }

            User registeredUser = _userRepository.GetUserById(id);
            if (registeredUser == null)
            {
                SendErrorMessage("Requested user data not found", ErrorCode.UserNotRegistrated, tokenMessage.Id);
                return;
            }

            Message<UserMessage> userMessage = new Message<UserMessage>("Response")
            {
                Id = tokenMessage.Id,
                Data = new UserMessage
                {
                    FirstName = registeredUser.FirstName,
                    LastName = registeredUser.LastName,
                    Token = tokenMessage.Data.Token
                }
            };

            NetMQMessage msg = userMessage.ToNetMQMessage();
            SendMessage(msg);
        }

        private void SendErrorMessage(string messageText, ErrorCode errorCode, NetMQFrame idFrame)
        {
            Message<ErrorMessage> errorMessage = new Message<ErrorMessage>("Error")
            {
                Data = new ErrorMessage
                {
                    MessageText = messageText,
                    ErrorCode = (int)errorCode
                },
                Id = idFrame
            };

            NetMQMessage msg = errorMessage.ToNetMQMessage();
            SendMessage(msg);
        }
    }
}