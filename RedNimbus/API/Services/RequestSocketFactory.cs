﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace RedNimbus.API.Services
{
    public static class RequestSocketFactory
    {
        private const string _facadeAddress = "tcp://127.0.0.1:8000";

        public static NetMQMessage SendRequest(NetMQMessage requestMessage)
        {
            RequestSocket requestSocket = new RequestSocket();
            requestSocket.Connect(_facadeAddress);
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