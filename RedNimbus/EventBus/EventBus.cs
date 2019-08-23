﻿using NetMQ;
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

        private const string _subscriberAdress = "";
        private const string _dealerAdress = "";

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
            _dealerSocket.Bind(_dealerAdress);
            _publisherSocket.Bind(_subscriberAdress);

            _dealerSocket.ReceiveReady += HandleReceiveEvent;

            _poller.Run();
        }

        public void Stop()
        {
            _dealerSocket.Unbind(_dealerAdress);
            _publisherSocket.Unbind(_subscriberAdress);

            _poller.Stop();
        }

        public void Publish(NetMQMessage msg)
        {
            _publisherSocket.SendMultipartMessage(msg);
        }
    }
}
