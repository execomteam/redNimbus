
using NetMQ;
using RedNimbus.Communication;
using RedNimbus.Domain;
using RedNimbus.Messages;
using System;
using ErrorCode = RedNimbus.Either.Enums.ErrorCode;

namespace UserService.Database
{
    public interface IUserCommunicationService
    {
        bool Validate(Message<UserMessage> message);
        Message<UserMessage> HandleRegisterUserRequest(NetMQMessage message);
        void HandleRegisterUserResponse(Message<UserMessage> userMessage, User user);
        void SendUserErrorMessage(string errorMessage, ErrorCode errCode, NetMQFrame id);
        Message<UserMessage> HandleAuthenticateUserRequest(NetMQMessage message);
        void HandleAuthenticateUserResponse(Message<UserMessage> userMessage, string Token);
        Message<TokenMessage> HandleGetUserRequest(NetMQMessage request);
        void HandleGetUserResponse(Message<TokenMessage> tokenMessage, User registeredUser);
        Message<TokenMessage> HandleDeactivateUserAccountRequest(NetMQMessage message);
        void HandleDeactivateUserAccountResponse(Message<TokenMessage> response);
    }
}
