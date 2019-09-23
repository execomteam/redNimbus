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
        private Action<NetMQMessage> _messageHandler;
        private bool _isRunning;
        private NetMQPoller _poller;

        public LogReceiver(string listenAddress, Action<NetMQMessage> messageHandler)
        {
            _listenAddress = listenAddress;
            _messageHandler = messageHandler;
            Start();
        }


        private void RecieiveReadyHandler(object sender, NetMQSocketEventArgs e)
        {
            NetMQMessage response = e.Socket.ReceiveMultipartMessage();
            _messageHandler(response);
        }

        public void Start()
        {
            if (!_isRunning)
            {
                Console.WriteLine("Start Listening...");
                _receiver = new DealerSocket();
                _poller = new NetMQPoller { _receiver };
                _receiver.ReceiveReady += RecieiveReadyHandler;
                _receiver.Bind(_listenAddress);
                _poller.Run();
                _isRunning = true;
            }
        }

        public void Stop()
        {
            if (_isRunning)
            {
                _receiver.Unbind(_listenAddress);
                _receiver.Close();
                _isRunning = false;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
