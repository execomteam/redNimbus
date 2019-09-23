using DTO;
using Microsoft.AspNetCore.Http;
using NetMQ;
using RedNimbus.API.Helper;
using RedNimbus.API.Services.Interfaces;
using RedNimbus.Communication;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using RedNimbus.Messages;
using System.IO;

namespace RedNimbus.API.Services
{
    public class LambdaService : BaseService, ILambdaService
    {
        public Either<IError, CreateLambdaDto> CreateLambda(CreateLambdaDto createLambdaDto)
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
                    OwnerId = "",
                    ImageId = "",
                    Guid = ""
                },
                Bytes = new NetMQFrame(memoryStream.ToArray())
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage());

            string responseTopic = response.First.ConvertToString();

            if (responseTopic.Equals("Response"))
            {
                Message<LambdaMessage> successMessage = new Message<LambdaMessage>(response);
                return new Right<IError, CreateLambdaDto>(LambdaConverter.LambdaMessageToDto(successMessage.Data));
            }

            return new Left<IError, CreateLambdaDto>(GetError(response));
        }

        public Either<IError, string> GetLambda(string lambdaId)
        {
            Message<LambdaMessage> message = new Message<LambdaMessage>("GetLambda")
            {
                Data = new LambdaMessage()
                {
                    Guid = lambdaId
                }
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage());

            string responseTopic = response.First.ConvertToString();

            if (responseTopic.Equals("Response"))
            {
                Message<LambdaResultMessage> successMessage = new Message<LambdaResultMessage>(response);

                return successMessage.Data.Result;
            }
            
            return new Left<IError, string>(GetError(response));
        }

        public Either<IError, byte[]> PostLambda(string lambdaId, IFormFile data)
        {
            var memoryStream = new MemoryStream();
            data.OpenReadStream().CopyTo(memoryStream);

            Message<LambdaMessage> message = new Message<LambdaMessage>("PostLambda")
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
