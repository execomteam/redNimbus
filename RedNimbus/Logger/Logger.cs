using Google.Protobuf;
using NetMQ;
using NetMQ.Sockets;
using RedNimbus.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LoggerService
{
    public class Logger
    {
    }

    public class LogSender : IDisposable
    {
        private DealerSocket _sender;

        public LogSender(string loggerEndpoint)
        {
            _sender = new DealerSocket();
            _sender.Connect(loggerEndpoint);
        }

        public void Dispose()
        {
            _sender.Close();
        }

        public void Send(string requestId, LogMessage log)
        {
            NetMQMessage message = new NetMQMessage();
            NetMQFrame idFrame = new NetMQFrame(requestId);
            NetMQFrame dataFrame; 

            using (var stream = new MemoryStream()) {
                log.WriteTo(stream);
                dataFrame = new NetMQFrame(stream.ToArray()); 
            }

            message.Append(idFrame);
            message.Append(dataFrame);

            _sender.SendMultipartMessage(message);
        }
    }

    public class LogReceiver
    {
        private DealerSocket _receiver;
        public LogReceiver()
        {

        }
    }
}
