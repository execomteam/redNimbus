
using Microsoft.EntityFrameworkCore;
using System;
using Google.Protobuf;
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
    class UserService
    {
        private IUserRepository             _userRepository;
        private ITokenManager               _tokenManager;
        private IUserCommunicationService   _userCommunicationService;
        private ILogSender                  _logSender;

        public UserService(IUserRepository userRepository, ITokenManager tokenManager, IUserCommunicationService userCommunicationService, ILogSender logSender)
        {
            this._userRepository            = userRepository;
            this._userCommunicationService  = userCommunicationService;
            this._tokenManager              = tokenManager;
            this._logSender                 = logSender;
            this.SubscribeToTopics();
        }

        private void SubscribeToTopics()
        {
            ((BaseService) _userCommunicationService).Subscribe("RegisterUser",             HandleRegisterUser);
            ((BaseService) _userCommunicationService).Subscribe("AuthenticateUser",         HandleAuthenticateUser);
            ((BaseService) _userCommunicationService).Subscribe("GetUser",                  HandleGetUser);
            ((BaseService) _userCommunicationService).Subscribe("DeactivateUserAccount",    HandleDeactivateUserAccount);
            ((BaseService) _userCommunicationService).Subscribe("ConfirmEmail",             ConfirmEmail);
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
                _userCommunicationService.SendUserErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
                return;
            }
            Guid id;
            try { 
                id = new Guid(tokenMessage.Data.Token);
                if (id.Equals(Guid.Empty))
                {
                    _userCommunicationService.SendUserErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
                    return;
                }
            }
            catch(Exception)
            {
                _userCommunicationService.SendUserErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
                return;
            }
           
            _userRepository.ActivateUserAccount(id);
            tokenMessage.Topic = "Response";
            var service = (BaseService)_userCommunicationService;
            service.SendMessage(tokenMessage.ToNetMQMessage());

        }

        private void HandleRegisterUser(NetMQMessage message)
        {
            Message<UserMessage> userMessage = _userCommunicationService.HandleRegisterUserRequest(message);

            if(userMessage == null)
              {
                return;
            }

            Log<UserMessage>(message, "UserService/HandleRegisterUser - Message received from Event bus", LogMessage.Types.LogType.Info);

            if (!_userCommunicationService.Validate(userMessage))
  {
                return;
            }

            User newUser = new User
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
                _userRepository.SaveUser(newUser);
                _userRepository.DeactivateUserAccount(newUser.Id);
                _userCommunicationService.HandleRegisterUserResponse(userMessage, newUser);
              Message<MailServiceMessage> mailMessage = new Message<MailServiceMessage>("SendMail")
                {
                    Data = new MailServiceMessage
                    {
                        MailTo = userMessage.Data.Email,
                        Subject = "Email confirmation for redNimbus",
                        Body = "Your confirmation link is: http://localhost:65001/api/user/emailConfirmation/" + newUser.Id.ToString()
                    }

                };
                var service = (BaseService)_userCommunicationService;
                service.SendMessage(mailMessage.ToNetMQMessage());
                Log<UserMessage>(message, "UserService/HandleRegisterUser - User registrated", LogMessage.Types.LogType.Info);
            }
            catch (DbUpdateException)
            {
                _userCommunicationService.SendUserErrorMessage("Email already exists", ErrorCode.EmailAlreadyUsed, userMessage.Id);
                Log<UserMessage>(message, "UserService/HandleRegisterUser - Email exist", LogMessage.Types.LogType.Info);
            }
            catch (Exception e)
            {
                _userCommunicationService.SendUserErrorMessage("Internal  server error", ErrorCode.InternalServerError, userMessage.Id);
                Log<UserMessage>(message, "UserService/HandleRegisterUser - Internal error", LogMessage.Types.LogType.Info);
            }
        }

        private void HandleAuthenticateUser(NetMQMessage message)
        {
            Message<UserMessage> userMessage = _userCommunicationService.HandleAuthenticateUserRequest(message);
            Log<UserMessage>(message, "UserService/HandleAuthenticateUser - Message received from Event bus", LogMessage.Types.LogType.Info);
            

            if (!Validation.IsEmailValid(userMessage.Data.Email))
            {
                _userCommunicationService.SendUserErrorMessage("Email or password are not valid!", ErrorCode.IncorrectEmailOrPassword, userMessage.Id);
                Log<UserMessage>(message, "UserService/HandleAuthenticateUser - Email not vaild", LogMessage.Types.LogType.Info);
            }

            if (!Validation.IsPasswordValid(userMessage.Data.Password))
            {
                _userCommunicationService.SendUserErrorMessage("Email or password are not valid!", ErrorCode.IncorrectEmailOrPassword, userMessage.Id);
                Log<UserMessage>(message, "UserService/HandleAuthenticateUser - Password not valid", LogMessage.Types.LogType.Info);
            }

            var email = userMessage.Data.Email;

            if (_userRepository.CheckIfExists(email))
            {
                var registeredUser = _userRepository.GetUserByEmail(userMessage.Data.Email);
              
                if(registeredUser.Password == HashHelper.ComputeHash(userMessage.Data.Password))
                {
                    var token = _tokenManager.GenerateToken(registeredUser.Id);
                    _userCommunicationService.HandleAuthenticateUserResponse(userMessage, token);
                   Log<UserMessage>(message, "UserService/HandleAuthenticateUser - Success", LogMessage.Types.LogType.Info);
                  return;
                }
            }
            _userCommunicationService.SendUserErrorMessage("Email or password are not valid", ErrorCode.IncorrectEmailOrPassword, userMessage.Id);
            Log<UserMessage>(message, "UserService/HandleAuthenticateUser - cant find user in db", LogMessage.Types.LogType.Info);
        }

        private void HandleGetUser(NetMQMessage message)
        {

            Message<TokenMessage> tokenMessage = _userCommunicationService.HandleGetUserRequest(message);
            Log<TokenMessage>(message, "UserService/HandleGetUser - Message received from Event bus", LogMessage.Types.LogType.Info);

            if (tokenMessage.Data.Token == null)
            {
                _userCommunicationService.SendUserErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
                Log<UserMessage>(message, "UserService/HandleGetUser - token is null", LogMessage.Types.LogType.Info);
                return;
            }

            Guid id = _tokenManager.ValidateToken(tokenMessage.Data.Token);

            if (id.Equals(Guid.Empty))
            {
                _userCommunicationService.SendUserErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
                Log<UserMessage>(message, "UserService/HandleGetUser - Invalid token", LogMessage.Types.LogType.Info);
                return;
            }

            User registeredUser = _userRepository.GetUserById(id);
            if (registeredUser == null)
            {
                _userCommunicationService.SendUserErrorMessage("Requested user data not found", ErrorCode.UserNotRegistrated, tokenMessage.Id);
                Log<UserMessage>(message, "UserService/HandleGetUser - can't find user in db", LogMessage.Types.LogType.Info);
                return;
            }

            _userCommunicationService.HandleGetUserResponse(tokenMessage, registeredUser);
            Log<UserMessage>(message, "UserService/HandleGetUser - Success", LogMessage.Types.LogType.Info);
        }

        private void HandleDeactivateUserAccount(NetMQMessage message)
        {
           Message<TokenMessage> tokenMessage = _userCommunicationService.HandleDeactivateUserAccountRequest(message);

            Guid id = _tokenManager.ValidateToken(tokenMessage.Data.Token);

            if (id.Equals(Guid.Empty))
            {
                _userCommunicationService.SendUserErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
                return;
            }

            if (_userRepository.GetUserById(id) == null)
            {
                _userCommunicationService.SendUserErrorMessage("Requested user data not found", ErrorCode.UserNotRegistrated, tokenMessage.Id);
                return;
            }

            _userRepository.DeactivateUserAccount(id);

            _userCommunicationService.HandleDeactivateUserAccountResponse(tokenMessage);
        }

        public void Start()
        {
           ((BaseService)_userCommunicationService).Start();
        }
    }
}
