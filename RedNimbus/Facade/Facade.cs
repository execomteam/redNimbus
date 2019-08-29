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
        private const string _facadeAddress = "tcp://*:8000";

        private RouterSocket _routerSocket;

        /// <summary>
        /// Facade constructor calling the base class constructor, and initializing the Router socket.
        /// </summary>
        public Facade() : base()
        {
            _routerSocket = new RouterSocket();

            Subscribe("Response", SendResponse);

            Poller.Add(_routerSocket);
        }

        /// <summary>
        /// Starts the service instance by attaching the sockets to specified endpoints.
        /// </summary>
        new public void Start()
        {
            if (IsStopped)
            {
                _routerSocket.Bind(_facadeAddress);

                _routerSocket.ReceiveReady += ReceiveRequestEventHandler;

                base.Start();
            }
        }

        /// <summary>
        /// Shut down the service and clean up resources.
        /// </summary>
        new public void Stop()
        {
            if (IsRunning)
            {
                base.Stop();

                try
                {
                    _routerSocket.Dispose();
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
            NetMQFrame idFrame = message[0];
            NetMQFrame topicFrame = message[2];
            NetMQFrame dataFrame = message[3];

            NetMQMessage dealerMessage = new NetMQMessage();
            dealerMessage.Append(topicFrame);
            dealerMessage.Append(idFrame);
            dealerMessage.Append(dataFrame);

            return dealerMessage;
        }

        /// <summary>
        /// Transforms the message received on the Subscriber socket to a message format
        /// that is sent over the Router socket.
        /// </summary>
        /// <param name="message">Message to be sent.</param>
        /// <returns>The transformed message.</returns>
        public NetMQMessage ToRouterMessage(NetMQMessage message)
        {
            NetMQMessage routerMessage = new NetMQMessage();

            NetMQFrame idFrame = message[1];
            NetMQFrame dataFrame = message[2];

            routerMessage.Append(idFrame);
            routerMessage.AppendEmptyFrame();
            routerMessage.Append(dataFrame);

            return routerMessage;
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
            _routerSocket.SendMultipartMessage(ToRouterMessage(message));
        }

       
    }
}
