using Google.Protobuf;
using NetMQ;
using NetMQ.Sockets;
using RedNimbus.Messages;
using System;
using System.IO;

namespace RedNimbus.LogLibrary
{
    public class LogSender : ILogSender
    {
        private DealerSocket _sender;
        string _loggerEndpoint;

        public LogSender(string loggerEndpoint)
        {
            _loggerEndpoint = loggerEndpoint;
            Start();
        }

        public bool IsRunning { get; }

        public void Start()
        {
            if (!IsRunning)
            {
                _sender = new DealerSocket();
                _sender.Connect(_loggerEndpoint);
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                _sender.Disconnect(_loggerEndpoint);
                _sender.Close();
            }
        }

        public void Send(Guid requestId, LogMessage log)
        {
            NetMQMessage message = new NetMQMessage();
            NetMQFrame idFrame = new NetMQFrame(requestId.ToByteArray());
            NetMQFrame dataFrame;

            using (var stream = new MemoryStream())
            {
                log.WriteTo(stream);
                dataFrame = new NetMQFrame(stream.ToArray());
            }

            message.Append(idFrame);
            message.Append(dataFrame);
            _sender.Options.Identity = requestId.ToByteArray();
            _sender.SendMultipartMessage(message);
        }
        public void Dispose()
        {
            Stop();
        }
    }
}
