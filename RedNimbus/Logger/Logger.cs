using NetMQ;
using NetMQ.Sockets;
using RedNimbus.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoggerService
{
    public class Logger
    {
    }

    public class LogSender
    {
        private DealerSocket _sender;
        public LogSender()
        {
            _sender = new DealerSocket("tcp://localhost:8010");
        }

        public void Send(string requestId, LogMessage log)
        {
            NetMQMessage message = new NetMQMessage();
            NetMQFrame idFrame = new NetMQFrame(requestId);
            NetMQFrame logFrame = new NetMQFrame();
        }
    }
}
