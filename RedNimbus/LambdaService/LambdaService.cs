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
        }

        private void HandleCreateLambda(NetMQMessage obj)
        {
            Message<LambdaMessage> message = new Message<LambdaMessage>(obj);
            throw new NotImplementedException();
        }
    }
}
