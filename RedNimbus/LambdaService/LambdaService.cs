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
            throw new NotImplementedException();
        }

        private void HandleGetLambda(NetMQMessage obj)
        {
            Message<GetLambdaMessage> message = new Message<GetLambdaMessage>(obj);
            throw new NotImplementedException();
        }
    }
}
