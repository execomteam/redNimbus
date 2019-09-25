using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using NetMQ;
using NetMQ.Sockets;
using RedNimbus.Communication;
using RedNimbus.LogLibrary;
using RedNimbus.Messages;

namespace RedNimbus.Facade
{
    public class Facade : BaseService
    {
        private const string _facadeAddress = "tcp://*:8000";

        private RouterSocket _routerSocket;

        private ILogSender _logger;

        /// <summary>
        /// Facade constructor calling the base class constructor, and initializing the Router socket.
        /// </summary>
        public Facade(string logEndpoint) : base()
        {
            _routerSocket = new RouterSocket();

            Subscribe("Response", SendResponse);
            Subscribe("Error", SendResponse);
            _logger = new LogSender(logEndpoint);
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
                try
                {
                    _routerSocket.Disconnect(_facadeAddress);
                    _routerSocket.Dispose();

                    base.Stop();
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
            NetMQFrame byteFrame = message[4];

            NetMQMessage dealerMessage = new NetMQMessage();
            dealerMessage.Append(topicFrame);
            dealerMessage.Append(idFrame);
            dealerMessage.Append(dataFrame);
            dealerMessage.Append(byteFrame);

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

            NetMQFrame topicFrame = message[0];
            NetMQFrame idFrame = message[1];
            NetMQFrame dataFrame = message[2];
            NetMQFrame byteFrame = message[3];

            routerMessage.Append(idFrame);
            routerMessage.AppendEmptyFrame();
            routerMessage.Append(topicFrame);
            routerMessage.Append(dataFrame);
            routerMessage.Append(byteFrame);

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
                LogMessage(new Guid(receivedMessage[0].ToByteArray()), "Facade/ReceiveRequestEventHandler - Message received from router");
                SendMessage(ToDealerMessage(receivedMessage));
                LogMessage(new Guid(receivedMessage[0].ToByteArray()), "Facade/ReceiveRequestEventHandler - Message sent to event bus");
            }
        }

        public void SendResponse(NetMQMessage message)
        {
            LogMessage(new Guid(message[1].ToByteArray()), "Facade/SendResponse - Message received from event bus");
            _routerSocket.SendMultipartMessage(ToRouterMessage(message));
            LogMessage(new Guid(message[1].ToByteArray()), "Facade/SendResponse - Message sent back to api gateway");
        }

        private void LogMessage(Guid id, string origin)
        {

            LogMessage logMessage = new LogMessage()
            {
                Origin = origin,
                Payload = "",
                Date = DateTime.Now.ToShortDateString().ToString(),
                Time = DateTime.Now.TimeOfDay.ToString(),
                Type = Messages.LogMessage.Types.LogType.Info
            };

            _logger.Send(id, logMessage);
        }

    }
}
