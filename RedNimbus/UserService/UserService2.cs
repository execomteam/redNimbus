using Microsoft.EntityFrameworkCore;
using NetMQ;
using RedNimbus.Communication;
using RedNimbus.Domain;
using RedNimbus.Either.Enums;
using RedNimbus.Messages;
using RedNimbus.TokenManager;
using RedNimbus.UserService.Helper;
using System;
using System.Collections.Generic;
using System.Text;
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

            User newUser = new User
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

        private void HandleAuthenticateUser(NetMQMessage obj)
        {
            throw new NotImplementedException();
        }

        private void HandleGetUser(NetMQMessage obj)
        {
            throw new NotImplementedException();
        }

        private void HandleDeactivateUserAccount(NetMQMessage obj)
        {
            throw new NotImplementedException();
        }
    }
}
