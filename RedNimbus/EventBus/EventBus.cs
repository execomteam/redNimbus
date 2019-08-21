using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimnus.EventBus
{
    public class EventBus
    {
        private DealerSocket _dealerSocket;
        private PublisherSocket _publisherSocket;
        private NetMQPoller _poller;

        private readonly string _subscriberAdress = "";
        private readonly string _dealerAdress = "";

        public EventBus()
        {
            _poller = new NetMQPoller { _dealerSocket };
            _dealerSocket.ReceiveReady += HandleReceiveEvent;
        }

        private void HandleReceiveEvent(object sender, NetMQSocketEventArgs e)
        {
            NetMQMessage msg = e.Socket.ReceiveMultipartMessage();
            Publish(msg);
        }

        public void Start()
        {
            _dealerSocket.Bind(_dealerAdress);
            _publisherSocket.Bind(_subscriberAdress);
            _poller.Run();

        }

        public void Stop()
        {
            _dealerSocket.Unbind(_dealerAdress);
            _publisherSocket.Unbind(_subscriberAdress);
            _poller.Stop();
        }

        //or template?
        public void Publish(NetMQMessage msg)
        {
            _publisherSocket.SendMultipartMessage(msg);
        }
    }
}
