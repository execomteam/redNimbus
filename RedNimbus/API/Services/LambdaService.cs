using DTO;
using NetMQ;
using RedNimbus.API.Helper;
using RedNimbus.API.Services.Interfaces;
using RedNimbus.Communication;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using RedNimbus.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.API.Services
{
    public class LambdaService : BaseService, ILambdaService
    {
        public Either<IError, CreateLambdaDto> CreateLambda(CreateLambdaDto createlambda)
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
                    OwnerId = "",
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

        public Either<IError, string> GetUserLambda(string token)
        {
            return "pogodio";
        }
    }
}
