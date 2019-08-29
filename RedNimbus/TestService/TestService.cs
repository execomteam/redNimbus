using NetMQ;
using RedNimbus.Communication;
using RedNimbus.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestService
{
    class TestService : BaseService
    {
        public TestService() : base()
        {
            Subscribe("TestTopic", HandleTestTopicAction);
        }

        private void HandleTestTopicAction(NetMQMessage obj)
        {
            Message<TestMessage> message = new Message<TestMessage>(obj);

            message.Data.Value = "Yay! :)";
            message.Topic = "Response";

            SendMessage(message.ToNetMQMessage());
        }
    }
}
