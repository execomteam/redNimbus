
using NetMQ;
using RedNimbus.Communication;
using RedNimbus.Domain;
using RedNimbus.Messages;
using RedNimbus.UserService.Helper;
using System;
using ErrorCode = RedNimbus.Either.Enums.ErrorCode;

namespace UserService.Database
{
    class UserCommunicationService : BaseService, IUserCommunicationService
    {
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
            if (!String.IsNullOrWhiteSpace(userMessage.Data.PhoneNumber) && !Validation.IsPhoneValid(userMessage.Data.PhoneNumber))
            {
                SendErrorMessage("Phone number wrong format", ErrorCode.PhoneNumberWrongFormat, userMessage.Id);
                return false;
            }

            return true;
        }

        public Message<UserMessage> HandleRegisterUserRequest(NetMQMessage message)
        {
            Message<UserMessage> userMessage = new Message<UserMessage>(message);
            if (!Validate(userMessage))
            {
                return null;
            }

            return userMessage;
        }

        public void HandleRegisterUserResponse(Message<UserMessage> userMessage, User user)
        {
            userMessage.Topic = "Response"; ;
            NetMQMessage response = userMessage.ToNetMQMessage();
            SendMessage(response);
        }

        public void SendUserErrorMessage(string errorMessage, ErrorCode errCode, NetMQFrame id)
        {
            SendErrorMessage(errorMessage, errCode, id);
        }

        public Message<UserMessage> HandleAuthenticateUserRequest(NetMQMessage message)
        {
            Message<UserMessage> userMessage = new Message<UserMessage>(message);

            if (!Validation.IsEmailValid(userMessage.Data.Email))
            {
                SendUserErrorMessage("Email or password are not valid!", ErrorCode.IncorrectEmailOrPassword, userMessage.Id);
            }

            if (!Validation.IsPasswordValid(userMessage.Data.Password))
            {
                SendUserErrorMessage("Email or password are not valid!", ErrorCode.IncorrectEmailOrPassword, userMessage.Id);
            }

            return userMessage;
        }

        public void HandleAuthenticateUserResponse(Message<UserMessage> userMessage, string Token)
        {
            Message<TokenMessage> tokenMessage = new Message<TokenMessage>("Response")
            {
                Id = userMessage.Id,
                Data = new TokenMessage
                {
                    Token = Token
                }
            };
            NetMQMessage response = tokenMessage.ToNetMQMessage();
            SendMessage(response);
        }

        public Message<TokenMessage> HandleGetUserRequest(NetMQMessage request)
        {
            Message<TokenMessage> tokenMessage = new Message<TokenMessage>(request);

            if (tokenMessage.Data.Token == null)
            {
                SendErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
                return null;
            }
            return tokenMessage;
        }

        public void HandleGetUserResponse(Message<TokenMessage> tokenMessage, User registeredUser)
        {
            Message<UserMessage> response = new Message<UserMessage>("Response")
            {
                Id = tokenMessage.Id,
                Data = new UserMessage
                {
                    FirstName = registeredUser.FirstName,
                    LastName = registeredUser.LastName,
                    Token = tokenMessage.Data.Token
                }
            };

            NetMQMessage msg = response.ToNetMQMessage();
            SendMessage(msg);
        }

        public Message<TokenMessage> HandleDeactivateUserAccountRequest(NetMQMessage message)
        {
            Message<TokenMessage> tokenMessage = new Message<TokenMessage>(message);

            if (tokenMessage.Data.Token == null)
            {
                SendErrorMessage("Requested user data not found", ErrorCode.UserNotFound, tokenMessage.Id);
                return null;
            }

            return tokenMessage;
        }

        public void HandleDeactivateUserAccountResponse(Message<TokenMessage> response)
        {
            response.Topic = "Response";
            NetMQMessage msg = response.ToNetMQMessage();
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
