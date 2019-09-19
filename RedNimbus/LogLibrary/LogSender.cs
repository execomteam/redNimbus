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
        private bool _isRunning;

        public LogSender(string loggerEndpoint)
        {
            _loggerEndpoint = loggerEndpoint;
            Start();
        }

        public void Start()
        {
            if (!_isRunning)
            {
                _sender = new DealerSocket();
                _sender.Connect(_loggerEndpoint);
                _isRunning = true;
            }
        }

        public void Stop()
        {
            if (_isRunning)
            {
                _sender.Disconnect(_loggerEndpoint);
                _sender.Close();
                _isRunning = false;
            }
        }

        public void SetIdentity(Guid id)
        {
            _sender.Options.Identity = id.ToByteArray();
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
