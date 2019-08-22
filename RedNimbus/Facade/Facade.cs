using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using RedNimbus.Communication;

namespace RedNimbus.Facade
{
    public class Facade : BaseService
    {
        // TODO: Set address
        private const string _requestAddress = "";

        private RouterSocket _routerSocket;
        private NetMQPoller _poller;

        /// <summary>
        /// Facade constructor calling the base class constructor, and initializing the Router socket.
        /// </summary>
        public Facade() : base()
        {
            _routerSocket = new RouterSocket();

            _poller = new NetMQPoller();
            _poller.Add(_routerSocket);
        }

        /// <summary>
        /// Starts the service instance by attaching the sockets to specified endpoints.
        /// </summary>
        new public void Start()
        {
            if(IsStopped)
            {
                base.Start();

                _routerSocket.Connect(_requestAddress);

                _routerSocket.ReceiveReady += ReceiveRequestEventHandler;

                _poller.Run();

                IsRunning = true;
            }

        }

        /// <summary>
        /// Temporarily pauses the service instance disconnecting the sockets from specified endpoints.
        /// </summary>
        new public void Pause()
        {
            if (IsRunning)
            {
                base.Pause();

                _routerSocket.Disconnect(_requestAddress);

                _poller.Stop();

                IsRunning = false;
            }
        }

        /// <summary>
        /// Shut down the service and clean up resources.
        /// </summary>
        new public void Stop()
        {
            if (IsRunning)
            {
                try
                {
                    _routerSocket.Dispose();

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
        /// Transforms the message received on the Router socket to a message format
        /// that is sent over the Dealer socket.
        /// </summary>
        /// <param name="message">Message to be sent.</param>
        /// <returns>The transformed message.</returns>
        public NetMQMessage ToDealerMessage(NetMQMessage message)
        {
            NetMQFrame idFrame = message.Pop();
            NetMQFrame topicFrame = message.Pop();

            message.Push(idFrame);
            message.Push(topicFrame);

            return message;
        }

        /// <summary>
        /// Transforms the message received on the Subscriber socket to a message format
        /// that is sent over the Router socket.
        /// </summary>
        /// <param name="message">Message to be sent.</param>
        /// <returns>The transformed message.</returns>
        public NetMQMessage ToRouterMessage(NetMQMessage message)
        {
            NetMQFrame topicFrame = message.Pop();
            NetMQFrame idFrame = message.Pop();

            message.Push(topicFrame);
            message.Push(idFrame);
            
            return message;
        }

        /// <summary>
        /// This method is invoked each time when the ReceiveReady event
        /// occurs on the Router socket that is attached to a NetMQPoller instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiveRequestEventHandler(object sender, NetMQSocketEventArgs e)
        {
            NetMQMessage receivedMessage = null;

            while (e.Socket.TryReceiveMultipartMessage(ref receivedMessage))
            {
                SendMessage(ToDealerMessage(receivedMessage));
            }
        }



        public void SendResponse(NetMQMessage message)
        {   
            // TODO: SUBSCRIBE TO RESPONSE EVENT

            _routerSocket.SendMultipartMessage(ToRouterMessage(message));
        }
    }
}
