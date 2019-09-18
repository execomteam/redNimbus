using Microsoft.EntityFrameworkCore;
using NetMQ;
using RedNimbus.Communication;
using RedNimbus.Domain;
using RedNimbus.Messages;
using RedNimbus.TokenManager;
using RedNimbus.UserService.Helper;
using System;
using UserService.Database;
using ErrorCode = RedNimbus.Either.Enums.ErrorCode;

namespace UserService
{
    class UserService2<T> where T:BaseService, IUserCommunicationService
    {
        private IUserRepository             _userRepository;
        private ITokenManager               _tokenManager;
        private T                           _userCommunicationService;

        public UserService2(IUserRepository userRepository, ITokenManager tokenManager, T userCommunicationService)
        {
            this._userRepository            = userRepository;
            this._userCommunicationService  = userCommunicationService;
            this._tokenManager              = tokenManager;
        }

        private void SubscribeToTopics()
        {
            _userCommunicationService.Subscribe("RegisterUser",             HandleRegisterUser);
            _userCommunicationService.Subscribe("AuthenticateUser",         HandleAuthenticateUser);
            _userCommunicationService.Subscribe("GetUser",                  HandleGetUser);
            _userCommunicationService.Subscribe("DeactivateUserAccount",    HandleDeactivateUserAccount);
        }

        private void HandleRegisterUser(NetMQMessage message)
        {
            Message<UserMessage> userMessage = _userCommunicationService.HandleRegisterUserRequest(message);

            if(userMessage == null)
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
                _userCommunicationService.HandleRegisterUserResponse(userMessage, newUser);
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

            var email = userMessage.Data.Email;

            if (_userRepository.CheckIfExists(email))
            {
                var registeredUser = _userRepository.GetUserByEmail(userMessage.Data.Email);
                if(registeredUser.Password == HashHelper.ComputeHash(userMessage.Data.Password))
                {
                    var token = _tokenManager.GenerateToken(registeredUser.Id);
                    _userCommunicationService.HandleAuthenticateUserResponse(userMessage, token);
                }
            }
            _userCommunicationService.SendUserErrorMessage("Email or password are not valid", ErrorCode.IncorrectEmailOrPassword, userMessage.Id);
        }

        private void HandleGetUser(NetMQMessage message)
        {
            Message<TokenMessage> tokenMessage = _userCommunicationService.HandleGetUserRequest(message);

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
    }
}
