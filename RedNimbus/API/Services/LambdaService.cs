using NetMQ;
using RedNimbus.API.Helper;
using RedNimbus.API.Services.Interfaces;
using RedNimbus.Communication;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using RedNimbus.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.API.Services
{
    public class LambdaService : BaseService, ILambdaService
    {
        public CreateLambdaDto LambdaMessageToDto(LambdaMessage message)
        {
            return new CreateLambdaDto
            {
                Name = message.Name,
                Trigger = message.Trigger,
                Runtime = message.Runtime,
                OwnerToken = message.OwnerId,
                ImageId = message.ImageId,
                Guid = message.Guid
            };
        }
        public Either<IError, CreateLambdaDto> CreateLambda(CreateLambdaDto createlambda)
        {
            Message<LambdaMessage> message = new Message<LambdaMessage>("CreateLambda")
            {
                Data = new LambdaMessage()
                {
                    Name = createlambda.Name,
                    Trigger = createlambda.Trigger,
                    Runtime = createlambda.Runtime,
                    OwnerId = createlambda.OwnerToken,
                    ImageId = createlambda.ImageId,
                    Guid = createlambda.Guid
                }
            };

            NetMQMessage temp = message.ToNetMQMessage();
            NetMQFrame topic = temp.Pop();
            NetMQFrame empty = temp.Pop();
            temp.Push(topic);

            NetMQMessage response = RequestSocketFactory.SendRequest(temp);

            string responseTopic = response.First.ConvertToString();

            if (responseTopic == "Response")
            {
                Message<LambdaMessage> successMessage = new Message<LambdaMessage>(response);
                return new Right<IError, CreateLambdaDto>(LambdaMessageToDto(successMessage.Data));
            }

            return new Left<IError, CreateLambdaDto>(GetError(response));
        }

       /* public Either<IError, string> GetLambda(string path, string token)
        {
            Message<LambdaMessage> message = new Message<LambdaMessage>("lambda/get")
            {
                Data = new LambdaMessage()
                {


                }
            };

            NetMQMessage temp = message.ToNetMQMessage();
            NetMQFrame topic = temp.Pop();
            NetMQFrame empty = temp.Pop();
            temp.Push(topic);

            NetMQMessage response = RequestSocketFactory.SendRequest(temp);
            string responseTopic = response.First.ConvertToString();

            if (responseTopic.Equals("Response"))
            {
                Message<LambdaMessage> successMessage = new Message<LambdaMessage>(response);
                return successMessage.Data;
                
            }
            return GetError(response);
        }*/
    }
    public class CreateLambdaDto
    {
        public string Name { get; set; }

        public LambdaMessage.Types.TriggerType Trigger { get; set; }

        public LambdaMessage.Types.RuntimeType Runtime { get; set; }

        public string OwnerToken { get; set; }

        public string ImageId { get; set; }

        public string Guid { get; set; }

        public byte[] File { get; set; }

    }
}
