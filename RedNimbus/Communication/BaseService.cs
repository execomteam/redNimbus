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
    public abstract class BaseService : IDisposable
    {
        private const string _publisherAddress = "";
        private const string _dealerAddress = "";

        private SubscriberSocket _subscriberSocket;
        private DealerSocket _dealerSocket;
        private NetMQPoller _poller;

        private Dictionary<string, Action<NetMQMessage>> _topicsToActions;

        public bool IsRunning { get; private set; }
        public bool IsStopped => !IsRunning;

        /// <summary>
        /// BaseService constructor initializing the Subscriber and Dealer sockets.
        /// </summary>
        protected BaseService()
        {
            _subscriberSocket = new SubscriberSocket();
            _dealerSocket = new DealerSocket();

            _poller = new NetMQPoller();
            _poller.Add(_subscriberSocket);

            _topicsToActions = new Dictionary<string, Action<NetMQMessage>>();
        }

        /// <summary>
        /// Starts the service instance by attaching the sockets to specified endpoints.
        /// </summary>
        public void Start()
        {
            if(IsStopped)
            {
                _subscriberSocket.Connect(_publisherAddress);
                _dealerSocket.Connect(_dealerAddress);

                _subscriberSocket.ReceiveReady += ReceiveMessageEventHandler;

                _poller.Run();

                IsRunning = true;
            }
        }

        /// <summary>
        /// Temporarily pauses the service instance disconnecting the sockets from specified endpoints.
        /// </summary>
        public void Pause()
        {
            if(IsRunning)
            {
                _subscriberSocket.Disconnect(_publisherAddress);
                _dealerSocket.Disconnect(_dealerAddress);

                _poller.Stop();

                IsRunning = false;
            }
        }

        /// <summary>
        /// Shut down the service and clean up resources.
        /// </summary>
        public void Stop()
        {
            if(IsRunning)
            {
                try
                {
                    _subscriberSocket.Dispose();
                    _dealerSocket.Dispose();

                    _poller.Dispose();
                }
                finally
                {
                    IsRunning = false;
                    NetMQConfig.Cleanup();
                }
            }
        }

        /// <summary>
        /// Subscribe the service to the specified topic to receive messages related to that topic.
        /// </summary>
        /// <param name="topic">String prefix of the topic.</param>
        /// <param name="action">Action that should be performed when a message for specified topic is received.</param>
        public void Subscribe(string topic, Action<NetMQMessage> action)
        {
            _subscriberSocket.Subscribe(topic);

            _topicsToActions.Add(topic, action);
        }

        /// <summary>
        /// Remove the service's subscription to the specified topic to stop receiving messages related to that topic.
        /// </summary>
        /// <param name="topic">String prefix of the topic.</param>
        public void Unsubscribe(string topic)
        {
            _subscriberSocket.Unsubscribe(topic);

            _topicsToActions.Remove(topic);
        }

        /// <summary>
        /// Sends the message to the event bus using dealer socket.
        /// </summary>
        /// <param name="message">Message to be sent.</param>
        public void SendMessage(NetMQMessage message)
        {
            _dealerSocket.SendMultipartMessage(message);
        }

        /// <summary>
        /// This method is invoked each time when the ReceiveReady event
        /// occurs on the subscriber socket that is attached to a NetMQPoller instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiveMessageEventHandler(object sender, NetMQSocketEventArgs e)
        {
            NetMQMessage receivedMessage = null;

            while(e.Socket.TryReceiveMultipartMessage(ref receivedMessage))
            {
                string topic = receivedMessage.Pop().ConvertToString();

                _topicsToActions.TryGetValue(topic, out Action<NetMQMessage> action);

                action?.Invoke(receivedMessage);
            }
        }

        #region IDisposable Support
        private bool _isDisposed = false;

        /// <summary>
        /// If disposing equals true, the method has been called directly or indirectly by a user's code.
        /// Managed and unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _subscriberSocket?.Dispose();
                    _dealerSocket?.Dispose();
                }

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Use this method for releasing unmanaged resources.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose() => Dispose(true);

        #endregion
    }
}
