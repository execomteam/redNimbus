using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.EventBus
{
    public class EventBus
    {
        private DealerSocket _dealerSocket;
        private PublisherSocket _publisherSocket;
        private NetMQPoller _poller;

        private const string _publisherAddress = "tcp://*:8081";
        private const string _dealerAddress = "tcp://*:8080";

        public EventBus()
        {
            _dealerSocket = new DealerSocket();
            _publisherSocket = new PublisherSocket();

            _poller = new NetMQPoller { _dealerSocket };
        }

        private void HandleReceiveEvent(object sender, NetMQSocketEventArgs e)
        {
            NetMQMessage msg = e.Socket.ReceiveMultipartMessage();
            Publish(msg);
        }

        public void Start()
        {
            _dealerSocket.Bind(_dealerAddress);
            _publisherSocket.Bind(_publisherAddress);

            _dealerSocket.ReceiveReady += HandleReceiveEvent;

            _poller.Run();
        }

        public void Pause()
        {
           _poller.Stop();
        }

        public void Stop()
        {
            try
            {
                _poller.Stop();
                _publisherSocket.Disconnect(_publisherAddress);
                _dealerSocket.Disconnect(_dealerAddress);

                _poller.Dispose();
                _publisherSocket.Dispose();
                _dealerSocket.Dispose();
            }
            finally
            {
                NetMQConfig.Cleanup();
            }
        }

        public void Publish(NetMQMessage msg)
        {
            _publisherSocket.SendMultipartMessage(msg);
        }
    }
}
