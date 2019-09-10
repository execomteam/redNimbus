using NetMQ;
using RedNimbus.API.Helper;
using RedNimbus.Communication;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.API.Services
{
    public class LambdaService : BaseService
    {
        public Either<IError, string> GetLambda()
        {
            Message<LambdaMessage> message = new Message<LambdaMessage>("lambda/get")
            {
                Data = new LambdaMessage()
                {
                    ...
                }
            }

            NetMQMessage temp = message.ToNetMQMessage();
            NetMQFrame topic = temp.Pop();
            NetMQFrame empty = temp.Pop();
            temp.Push(topic);

            NetMQMessage response = RequestSocketFactory.SendRequest(temp);
            string responseTopic = response.First.ConvertToString();

            if (responseTopic.Equals("Response"))
            {
                Message<LambdaMessage> successMessage = new Message<LambdaMessage>(response);
                return successMessage;
                
            }
            return GetError(response);
        }
    }
}
