using System;
using System.Collections.Generic;
using NetMQ;
using NetMQ.Sockets;
using RedNimbus.Either;
using RedNimbus.Messages;

namespace RedNimbus.Communication
{
    /// <summary>
    /// Defines the most basic functionalities of a service.
    /// </summary>
    public abstract class BaseService : IDisposable
    {
        private const string _publisherAddress = "tcp://127.0.0.1:8081";
        private const string _dealerAddress = "tcp://127.0.0.1:8080";

        protected const string _returnTopic = "Response";

        private SubscriberSocket _subscriberSocket;
        private DealerSocket _dealerSocket;
        private NetMQPoller _poller;

        private IDictionary<string, Action<NetMQMessage>> _topicsToActions;


        /// <summary>
        /// Use this NetMQPoller instance to observe sockets.
        /// </summary>
        protected NetMQPoller Poller
        {
            get { return _poller; }
            private set { }
        }

        /// <summary>True if the service is up and running, else False.</summary>
        public bool IsRunning { get; protected set; }

        /// <summary>False if the service is up and running, else True.</summary>
        public bool IsStopped => !IsRunning;

        /// <summary>
        /// BaseService constructor initializing the Subscriber and Dealer sockets.
        /// </summary>
        protected BaseService()
        {
            _subscriberSocket = new SubscriberSocket();
            _dealerSocket = new DealerSocket();

            _poller = new NetMQPoller { _subscriberSocket };

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
        /// Temporarily pauses the service instance by stopping the poller.
        /// </summary>
        public void Pause()
        {
            if(IsRunning)
            {
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
                    _poller.Stop();
                    _subscriberSocket.Disconnect(_publisherAddress);
                    _dealerSocket.Disconnect(_dealerAddress);

                    _poller.Dispose();
                    _subscriberSocket.Dispose();
                    _dealerSocket.Dispose();
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
        /// <param name="topic">String prefix of the topic.</paramfg>
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

        protected void SendErrorMessage(string messageText, Either.Enums.ErrorCode errorCode, NetMQFrame idFrame)
        {
            Message<ErrorMessage> errorMessage = new Message<ErrorMessage>("Error");

            errorMessage.Data.MessageText = messageText;
            errorMessage.Data.ErrorCode = (int)errorCode;
            errorMessage.Id = idFrame;

            NetMQMessage msg = errorMessage.ToNetMQMessage();
            SendMessage(msg);
        }

        /// <summary>
        /// This method is invoked each time when the ReceiveReady event
        /// occurs on the subscriber socket that is attached to a NetMQPoller instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiveMessageEventHandler(object sender, NetMQSocketEventArgs e)
        {

            // TODO: Wildcard topic
            NetMQMessage receivedMessage = null;

            while(e.Socket.TryReceiveMultipartMessage(ref receivedMessage))
            {
                string topic = receivedMessage.First.ConvertToString();

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
