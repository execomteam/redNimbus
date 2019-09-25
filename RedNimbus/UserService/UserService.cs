
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

        public UserService(IUserRepository userRepository, ITokenManager tokenManager, IUserCommunicationService userCommunicationService)
        {
            this._userRepository            = userRepository;
            this._userCommunicationService  = userCommunicationService;
            this._tokenManager              = tokenManager;
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

            if (userMessage == null)
            {
                return;
            }
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
            }
            catch (DbUpdateException)
            {
                _userCommunicationService.SendUserErrorMessage("Email already exists", ErrorCode.EmailAlreadyUsed, userMessage.Id);
            }
            catch (Exception)
            {
                _userCommunicationService.SendUserErrorMessage("Internal  server error", ErrorCode.InternalServerError, userMessage.Id);
            }
        }

        private void HandleAuthenticateUser(NetMQMessage message)
        {
            Message<UserMessage> userMessage = _userCommunicationService.HandleAuthenticateUserRequest(message);            

            if (!Validation.IsEmailValid(userMessage.Data.Email))
            {
                _userCommunicationService.SendUserErrorMessage("Email or password are not valid!", ErrorCode.IncorrectEmailOrPassword, userMessage.Id);
            }

            if (!Validation.IsPasswordValid(userMessage.Data.Password))
            {
                _userCommunicationService.SendUserErrorMessage("Email or password are not valid!", ErrorCode.IncorrectEmailOrPassword, userMessage.Id);
            }

            var email = userMessage.Data.Email;

            if (_userRepository.CheckIfExists(email))
            {
                var registeredUser = _userRepository.GetUserByEmail(userMessage.Data.Email);

                if (registeredUser.Password == HashHelper.ComputeHash(userMessage.Data.Password))
                {
                    var token = _tokenManager.GenerateToken(registeredUser.Id);
                    _userCommunicationService.HandleAuthenticateUserResponse(userMessage, token);
                    return;
                }
            }
            _userCommunicationService.SendUserErrorMessage("Email or password are not valid", ErrorCode.IncorrectEmailOrPassword, userMessage.Id);
        }

        private void HandleGetUser(NetMQMessage message)
        {

            Message<TokenMessage> tokenMessage = _userCommunicationService.HandleGetUserRequest(message);

            if (tokenMessage.Data.Token == null)
            {
                _userCommunicationService.SendUserErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
                return;
            }

            Guid id = _tokenManager.ValidateToken(tokenMessage.Data.Token);

            if (id.Equals(Guid.Empty))
            {
                _userCommunicationService.SendUserErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
                return;
            }

            User registeredUser = _userRepository.GetUserById(id);
            if (registeredUser == null)
            {
                _userCommunicationService.SendUserErrorMessage("Requested user data not found", ErrorCode.UserNotRegistrated, tokenMessage.Id);
                return;
            }

            _userCommunicationService.HandleGetUserResponse(tokenMessage, registeredUser);
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
