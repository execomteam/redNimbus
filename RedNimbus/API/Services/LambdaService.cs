using DTO;
using NetMQ;
using RedNimbus.API.Helper;
using RedNimbus.API.Services.Interfaces;
using RedNimbus.Communication;
using RedNimbus.Domain;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using RedNimbus.Messages;
using System.Collections.Generic;
using System.IO;

namespace RedNimbus.API.Services
{
    public class LambdaService : BaseService, ILambdaService
    {
        public Either<IError, CreateLambdaDto> CreateLambda(CreateLambdaDto createlambda, string token)
        {
            var memoryStream = new MemoryStream();
            createlambda.File.OpenReadStream().CopyTo(memoryStream);


            Message<LambdaMessage> message = new Message<LambdaMessage>("CreateLambda")
            {
                Data = new LambdaMessage()
                {
                    Name = createlambda.Name,
                    Trigger = createlambda.Trigger,
                    Runtime = createlambda.Runtime,
                    OwnerId = token,
                    ImageId = "",
                    Guid = ""
                },
                Bytes = new NetMQFrame(memoryStream.ToArray())
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage());

            string responseTopic = response.First.ConvertToString();

            if (responseTopic == "Response")
            {
                Message<LambdaMessage> successMessage = new Message<LambdaMessage>(response);
                return new Right<IError, CreateLambdaDto>(LambdaConverter.LambdaMessageToDto(successMessage.Data));
            }

            return new Left<IError, CreateLambdaDto>(GetError(response));
        }

        public Either<IError, string> GetLambda(string lambdaId, string token)
        {
            Message<GetLambdaMessage> message = new Message<GetLambdaMessage>("GetLambda")
            {
                Data = new GetLambdaMessage()
                {
                    Token = token,
                    LambdaId = lambdaId
                }
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage());

            string responseTopic = response.First.ConvertToString();

            if (responseTopic == "Response")
            {
                Message<GetLambdaMessage> successMessage = new Message<GetLambdaMessage>(response);
                return successMessage.Data.Result;
            }
            
            return new Left<IError, string>(GetError(response));
        }

        Either<IError, List<Lambda>> GetLambdas(string token)
        {
            Message<ListLambdasMessage> message = new Message<ListLambdasMessage>("GetLambda")
            {
                Data = new ListLambdasMessage()
                {
                    Token = token
                }
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage());

            string responseTopic = response.First.ConvertToString();

            if (responseTopic == "Response")
            {
                Message<ListLambdasMessage> successMessage = new Message<ListLambdasMessage>(response);
                List<Lambda> lambdas = new List<Lambda>();
                foreach(LambdaMessage l in successMessage.Data.Lambdas)
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
    }
}
