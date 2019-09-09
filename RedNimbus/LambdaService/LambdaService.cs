using NetMQ;
using RedNimbus.Communication;
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
            throw new NotImplementedException();
        }
    }
}
