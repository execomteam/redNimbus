using NetMQ;
using RedNimbus.Communication;
using RedNimbus.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.LambdaService
{
    class LambdaService : BaseService
    {
        public LambdaService() : base()
        {
            Subscribe("CreateLambda", HandleCreateLambda);
            Subscribe("GetLambda", HandleGetLambda);
        }

        private void HandleCreateLambda(NetMQMessage obj)
        {
            Message<LambdaMessage> message = new Message<LambdaMessage>(obj);

            Guid guid = Utility.CreateLambda(message);

            message.Topic = "Response";
            message.Bytes = new NetMQFrame("");
            message.Data.Guid = guid.ToString();

            SendMessage(message.ToNetMQMessage());
        }

        private void HandleGetLambda(NetMQMessage obj)
        {
            Message<GetLambdaMessage> message = new Message<GetLambdaMessage>(obj);
            message.Topic = "Response";
            SendMessage(message.ToNetMQMessage());
        }
    }
}
