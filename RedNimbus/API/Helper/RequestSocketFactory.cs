using System;
using NetMQ;
using NetMQ.Sockets;

namespace RedNimbus.API.Helper
{
    public static class RequestSocketFactory
    {
        private const string _facadeAddress = "tcp://127.0.0.1:8000";

        public static NetMQMessage SendRequest(NetMQMessage requestMessage)
        {
            using (var requestSocket = new RequestSocket(_facadeAddress))
            {
                requestSocket.SendMultipartMessage(requestMessage);

                NetMQMessage responseMessage = null;
                TimeSpan timeSpan = new TimeSpan(0, 0, 15);
                requestSocket.TryReceiveMultipartMessage(timeSpan, ref responseMessage);

                requestSocket.Disconnect(_facadeAddress);
                requestSocket.Dispose();

                return responseMessage;
            }
        }
    }
}
