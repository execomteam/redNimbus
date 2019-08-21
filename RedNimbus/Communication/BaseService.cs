using System;
using System.Collections.Generic;
using System.Text;
using NetMQ;
using NetMQ.Sockets;

namespace RedNimbus.Communication
{
    /// <summary>
    /// Defines the most basic functionalities of a service.
    /// </summary>
    public abstract class BaseService
    {
        private readonly string _publisherAddress = "";
        private readonly string _dealerAddress = "";

        private SubscriberSocket _subscriberSocket;
        private DealerSocket _dealerSocket;

        /// <summary>
        /// Starts the service instance.
        /// </summary>
        public void Start()
        {
            _subscriberSocket.Connect(_publisherAddress);
            _dealerSocket.Connect(_dealerAddress);
        }

        /// <summary>
        /// Temporarily stops the service instance.
        /// </summary>
        public void Stop()
        {
            _subscriberSocket.Disconnect(_publisherAddress);
            _dealerSocket.Disconnect(_dealerAddress);

            // TODO: IDisposable
        }

        /// <summary>
        /// Subscribe the service to the specified topic to receive messages related to that topic.
        /// </summary>
        /// <param name="topic">String prefix of the topic.</param>
        public void Subscribe(string topic)
        {
            _subscriberSocket.Subscribe(topic);
        }

        /// <summary>
        /// Remove the service's subscription to the specified topic to stop receiving messages related to that topic.
        /// </summary>
        /// <param name="topic">String prefix of the topic.</param>
        public void Unsubscribe(string topic)
        {
            _subscriberSocket.Unsubscribe(topic);
        }

        public void SendMessage(NetMQMessage message)
        {
            // TODO: Send event to event bus using dealer socket

            _dealerSocket.SendMultipartMessage(message);
        }
    }
}
