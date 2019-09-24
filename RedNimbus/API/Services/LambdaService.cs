using DTO;
using Microsoft.AspNetCore.Http;
using NetMQ;
using RedNimbus.API.Helper;
using RedNimbus.API.Services.Interfaces;
using RedNimbus.Communication;
using RedNimbus.Domain;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using RedNimbus.Messages;
using System;
using System.Collections.Generic;
using System.IO;

namespace RedNimbus.API.Services
{ 
    public class LambdaService : BaseService, ILambdaService
    {
        public Either<IError, CreateLambdaDto> Create(CreateLambdaDto createLambdaDto, string token, Guid requestId)
        {
            var memoryStream = new MemoryStream();
            createLambdaDto.File.OpenReadStream().CopyTo(memoryStream);

            Message<LambdaMessage> message = new Message<LambdaMessage>("CreateLambda")
            {
                Data = new LambdaMessage()
                {
                    Name = createLambdaDto.Name,
                    Trigger = createLambdaDto.Trigger,
                    Runtime = createLambdaDto.Runtime,
                    OwnerId = token,
                    ImageId = "",
                    Guid = ""
                },
                Bytes = new NetMQFrame(memoryStream.ToArray())
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage(), requestId);

            string responseTopic = response.First.ConvertToString();

            if (responseTopic.Equals("Response"))
            {
                Message<LambdaMessage> successMessage = new Message<LambdaMessage>(response);
                return new Right<IError, CreateLambdaDto>(LambdaConverter.LambdaMessageToDto(successMessage.Data));
            }

            return new Left<IError, CreateLambdaDto>(GetError(response));
        }

        public Either<IError, List<Lambda>> GetAll(string token, Guid requestId)
        {
            // TODO: Re-check this

            Message<ListLambdasMessage> message = new Message<ListLambdasMessage>("GetUserLambdas")
            {
                Data = new ListLambdasMessage()
                {
                    Token = token
                }
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage(), requestId);

            string responseTopic = response.First.ConvertToString();

            if (responseTopic == "Response")
            {
                Message<ListLambdasMessage> successMessage = new Message<ListLambdasMessage>(response);
                List<Lambda> lambdas = new List<Lambda>();
                foreach (LambdaMessage l in successMessage.Data.Lambdas)
                {
                    Lambda lmd = new Lambda();
                    lmd.Name = l.Name;
                    lmd.Guid = l.Guid;
                    lmd.Trigger = l.Trigger;
                    lmd.Runtime = l.Runtime;
                    lambdas.Add(lmd);
                }

                return new Right<IError, List<Lambda>>(lambdas);
            }

            return new Left<IError, List<Lambda>>(GetError(response));

        }

        public Either<IError, string> ExecuteGetLambda(string lambdaId, string token, Guid requestId)
        {
            Message<LambdaMessage> message = new Message<LambdaMessage>("ExecuteGetLambda")
            {
                Data = new LambdaMessage()
                {
                    Guid = lambdaId
                }
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage(), requestId);

            string responseTopic = response.First.ConvertToString();

            if (responseTopic.Equals("Response"))
            {
                Message<LambdaResultMessage> successMessage = new Message<LambdaResultMessage>(response);

                return successMessage.Data.Result;
            }
            
            return new Left<IError, string>(GetError(response));
        }

        public Either<IError, byte[]> ExecutePostLambda(string lambdaId, IFormFile data, Guid requestId)
        {
            var memoryStream = new MemoryStream();
            data.OpenReadStream().CopyTo(memoryStream);

            Message<LambdaMessage> message = new Message<LambdaMessage>("ExecutePostLambda")
            {
                Data = new LambdaMessage()
                {
                    Guid = lambdaId
                },
                Bytes = new NetMQFrame(memoryStream.ToArray())
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage());

            string responseTopic = response.First.ConvertToString();

            if (responseTopic.Equals("Response"))
            {
                Message<LambdaResultMessage> successMessage = new Message<LambdaResultMessage>(response);

                return successMessage.Bytes.ToByteArray();
            }

            return new Left<IError, byte[]>(GetError(response));
        }
    }
}
