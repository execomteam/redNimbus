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
            Subscribe("DeactivateUserAccount", HandleDeactivateUserAccount);
            Subscribe("ConfirmEmail", ConfirmEmail);

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

        private void Log<T>(NetMQMessage message, string origin, LogMessage.Types.LogType type) where T: IMessage, new()
        {
            T payload = new T();
            payload.MergeFrom(message[2].ToByteArray());

            LogMessage logMessage = new LogMessage()
            {
                Origin = origin,
                Payload = "**** " + payload.ToString(),
                Date = DateTime.Now.ToShortDateString(),
                Time = DateTime.Now.TimeOfDay.ToString(),
                Type = type
            };

            _logSender.Send(new Guid(message[1].ToByteArray()), logMessage);
        }
        
        private void ConfirmEmail(NetMQMessage message)
        {
            Message<TokenMessage> tokenMessage = new Message<TokenMessage>(message);

            if (tokenMessage.Data.Token == null)
            {
                SendErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
                return;
            }
            Guid id;
            try { 
                id = new Guid(tokenMessage.Data.Token);
                if (id.Equals(Guid.Empty))
                {
                    SendErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
                    return;
                }
            }
            catch(Exception)
            {
                SendErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
                return;
            }
           
            _userRepository.ActivateUserAccount(id);
            tokenMessage.Topic = "Response";
            SendMessage(tokenMessage.ToNetMQMessage());

        }

        private void HandleRegisterUser(NetMQMessage message)
        {
            Log<UserMessage>(message, "UserService/HandleRegisterUser - Message received from Event bus", LogMessage.Types.LogType.Info);

            Message<UserMessage> userMessage = new Message<UserMessage>(message);

            if (!Validate(userMessage))
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
                PhoneNumber = userMessage.Data.PhoneNumber
            };

            try
            {
                this._userRepository.SaveUser(user);

                userMessage.Topic = "Response";

                Message<MailServiceMessage> mailMessage = new Message<MailServiceMessage>("SendMail")
                {
                    Data = new MailServiceMessage
                    {
                        MailTo = userMessage.Data.Email,
                        Subject = "Email confirmation for redNimbus",
                        Body = "Your confirmation link is: http://localhost:65001/api/user/emailConfirmation/" + user.Id.ToString()
                    }

                };
                SendMessage(mailMessage.ToNetMQMessage()); 

                NetMQMessage msg = userMessage.ToNetMQMessage();
                SendMessage(msg);
                Log<UserMessage>(message, "UserService/HandleRegisterUser - User registrated", LogMessage.Types.LogType.Info);
            }
            catch (DbUpdateException)
            {
                SendErrorMessage("Email already exists.", ErrorCode.EmailAlreadyUsed, userMessage.Id);
                Log<UserMessage>(message, "UserService/HandleRegisterUser - Email exist", LogMessage.Types.LogType.Info);
            }
            catch (Exception e)
            {
                SendErrorMessage("Internal server error.", ErrorCode.InternalServerError, userMessage.Id);
                Log<UserMessage>(message, "UserService/HandleRegisterUser - Internal error", LogMessage.Types.LogType.Info);
            }
        }

        private void HandleAuthenticateUser(NetMQMessage message)
        {
            Log<UserMessage>(message, "UserService/HandleAuthenticateUser - Message received from Event bus", LogMessage.Types.LogType.Info);

            Message<UserMessage> userMessage = new Message<UserMessage>(message);

            if (!Validation.IsEmailValid(userMessage.Data.Email))
            {
                SendErrorMessage("Email or password are not valid!", ErrorCode.IncorrectEmailOrPassword, userMessage.Id);
                Log<UserMessage>(message, "UserService/HandleAuthenticateUser - Email not vaild", LogMessage.Types.LogType.Info);
            }

            if (!Validation.IsPasswordValid(userMessage.Data.Password))
            {
                SendErrorMessage("Email or password are not valid!", ErrorCode.IncorrectEmailOrPassword, userMessage.Id);
                Log<UserMessage>(message, "UserService/HandleAuthenticateUser - Password not valid", LogMessage.Types.LogType.Info);
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
                    Log<UserMessage>(message, "UserService/HandleAuthenticateUser - Success", LogMessage.Types.LogType.Info);
                    return;
                }
            }

            SendErrorMessage("Email or password are not valid!", ErrorCode.IncorrectEmailOrPassword, userMessage.Id);
            Log<UserMessage>(message, "UserService/HandleAuthenticateUser - cant find user in db", LogMessage.Types.LogType.Info);
        }



        private void HandleGetUser(NetMQMessage message)
        {
            Log<TokenMessage>(message, "UserService/HandleGetUser - Message received from Event bus", LogMessage.Types.LogType.Info);

            Message<TokenMessage> tokenMessage = new Message<TokenMessage>(message);

            if (tokenMessage.Data.Token == null)
            {
                SendErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
                Log<UserMessage>(message, "UserService/HandleGetUser - token is null", LogMessage.Types.LogType.Info);
                return;
            }

            Guid id = _tokenManager.ValidateToken(tokenMessage.Data.Token);
            if (id.Equals(Guid.Empty))
            {
                SendErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
                Log<UserMessage>(message, "UserService/HandleGetUser - Invalid token", LogMessage.Types.LogType.Info);
                return;
            }

            User registeredUser = _userRepository.GetUserById(id);
            if (registeredUser == null)
            {
                SendErrorMessage("Requested user data not found", ErrorCode.UserNotRegistrated, tokenMessage.Id);
                Log<UserMessage>(message, "UserService/HandleGetUser - can't find user in db", LogMessage.Types.LogType.Info);
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
            Log<UserMessage>(message, "UserService/HandleGetUser - Success", LogMessage.Types.LogType.Info);
        }

        private void HandleDeactivateUserAccount(NetMQMessage message)
        {
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

            if (_userRepository.GetUserById(id) == null)
            {
                SendErrorMessage("Requested user data not found", ErrorCode.UserNotRegistrated, tokenMessage.Id);
                return;
            }

            _userRepository.DeactivateUserAccount(id);

            Message<TokenMessage> userMessage = new Message<TokenMessage>("Response");
            NetMQMessage msg = tokenMessage.ToNetMQMessage();
            SendMessage(msg);
        }
    }
}