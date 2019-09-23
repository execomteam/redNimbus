﻿using RedNimbus.API.Services.Interfaces;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using RedNimbus.Messages;
using RedNimbus.Communication;
using RedNimbus.DTO;
using RedNimbus.Domain;
using NetMQ;
using RedNimbus.API.Helper;
using System;

namespace RedNimbus.API.Services
{
    public class UserService : BaseService, IUserService
    {
        public Either<IError, User> RegisterUser(User user, Guid id)
        {
            Message<UserMessage> message = new Message<UserMessage>("RegisterUser")
            {
                Data = new UserMessage()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = user.Password
                }
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage(), id);

            string responseTopic = response.First.ConvertToString();

            if (responseTopic.Equals("Response"))
            {
                Message<UserMessage> successMessage = new Message<UserMessage>(response);

                return new Right<IError, User>(user);
            }

            return new Left<IError, User>(GetError(response));
        }
        
        public Either<IError, Empty> deactivateUserAccount(string token)
        {
            Message<TokenMessage> message = new Message<TokenMessage>("DeactivateUserAccount")
            {
                Data = new TokenMessage()
                {
                    Token = token
                }
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage());
            string responseTopic = response.First.ConvertToString();

            if (responseTopic.Equals("Response"))
            {
                Message<UserMessage> successMessage = new Message<UserMessage>(response);
                return new Right<IError, Empty>(new Empty());
            }

            return new Left<IError, Empty>(GetError(response));
        }

        public Either<IError, KeyDto> Authenticate(User user, Guid requestId)
        {
            Message<UserMessage> message = new Message<UserMessage>("AuthenticateUser")
            {
                Data = new UserMessage()
                {
                    Email = user.Email,
                    Password = user.Password
                }
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage(), requestId);

            string responseTopic = response.First.ConvertToString();

            if (responseTopic.Equals("Response"))
            {
                Message<TokenMessage> successMessage = new Message<TokenMessage>(response);

                KeyDto keyDto = new KeyDto();
                keyDto.Key = successMessage.Data.Token;

                return new Right<IError, KeyDto>(keyDto);
            }

            return new Left<IError, KeyDto>(GetError(response));
        }

        public Either<IError, User> GetUserByToken(string token, Guid id)
        {
            Message<TokenMessage> message = new Message<TokenMessage>("GetUser")
            {
                Data = new TokenMessage()
                {
                    Token = token
                }
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage(), id);

            string responseTopic = response.First.ConvertToString();

            if (responseTopic.Equals("Response"))
            {
                Message<UserMessage> successMessage = new Message<UserMessage>(response);

                User user = new User
                {
                    FirstName = successMessage.Data.FirstName,
                    LastName = successMessage.Data.LastName,
                    Key = successMessage.Data.Token
                };

                return new Right<IError, User>(user);
            }

            return new Left<IError, User>(GetError(response));
        }

        public Either<IError, bool> EmailConfirmation(string token)
        {
            Message<TokenMessage> message = new Message<TokenMessage>("ConfirmEmail")
            {
                Data = new TokenMessage()
                {
                    Token = token
                }
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage());

            string responseTopic = response.First.ConvertToString();

            if (responseTopic.Equals("Response"))
            {
                return new Right<IError, bool>(true);
            }

            return new Left<IError, bool>(GetError(response));
        }
    }
}
