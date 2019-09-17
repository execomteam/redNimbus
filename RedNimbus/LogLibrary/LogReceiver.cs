using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedNimbus.LogLibrary
{

    public class LogReceiver : ILogReceiver
    {
        private DealerSocket _receiver;
        private string _listenAddress;
        Action<NetMQMessage> _messageHandler;

        private NetMQPoller _poller;

        public LogReceiver(string listenAddress, Action<NetMQMessage> messageHandler)
        {
            _listenAddress = listenAddress;
            _messageHandler = messageHandler;
            Start();
        }

        public bool IsRunning { get; }

        private void RecieiveReadyHandler(object sender, NetMQSocketEventArgs e)
        {
            NetMQMessage response = e.Socket.ReceiveMultipartMessage();
            _messageHandler(response);
        }

        public void Start()
        {
            if (!IsRunning)
            {
                Console.WriteLine("Start Listening...");
                _receiver = new DealerSocket();
                _poller = new NetMQPoller { _receiver };
                _receiver.ReceiveReady += RecieiveReadyHandler;
                _receiver.Bind(_listenAddress);
                _poller.Run();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                _receiver.Unbind(_listenAddress);
                _receiver.Close();
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
