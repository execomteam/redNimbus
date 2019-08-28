using Microsoft.AspNetCore.Mvc;
using NetMQ;
using NetMQ.Sockets;
using RedNimbus.API.Models;
using RedNimbus.API.Services.Interfaces;
using RedNimbus.DTO.Interfaces;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RedNimbus.Messages;
using RedNimbus.Communication;

namespace RedNimbus.API.Services
{
    public class CommunicationService : ICommunicationService
    {
        
        private const string _facadeAddress = "tcp://127.0.0.1:8000";

        private RequestSocket _requestSocket;


        public string SendTestRequest()
        {
            //TODO 1: Create NetMQMessage
            //TODO 2: Send using request socket
            //TODO 3: wait for response message

            NetMQMessage testMessage = new NetMQMessage();
            testMessage.PushEmptyFrame();
            testMessage.Push(new NetMQFrame("TestTopic"));

            Message<TestMessage> message = new Message<TestMessage>(testMessage);
            message.Data.Value = "Test Message Data Value";

            _requestSocket.SendMultipartMessage(message.ToNetMQMessage());

            NetMQMessage receivedMessage = null;

            string returnData = "No Value";

            receivedMessage = _requestSocket.ReceiveMultipartMessage();
            
            returnData = receivedMessage[2].ConvertToString();

            

            return returnData;
        }



        #region httpService

        private readonly string _address;

        public CommunicationService(string address) {
            _address = address;

            // TODO
            _requestSocket = new RequestSocket();
            _requestSocket.Connect(_facadeAddress);
        }  

        public async Task<Either<IError, TSuccess>> Send<TRequest, TSuccess>(string path, TRequest data)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_address);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            Either<IError, TSuccess> result = null;
            HttpResponseMessage response = await client.PostAsJsonAsync(path, data);

            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsAsync<TSuccess>();
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                result = await response.Content.ReadAsAsync<FormatError>();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                result = await response.Content.ReadAsAsync<NotFoundError>();
            }
            else if (response.StatusCode == HttpStatusCode.NotAcceptable)
            {
                result = await response.Content.ReadAsAsync<AuthenticationError>();
            }
            else 
            {
                result = await response.Content.ReadAsAsync<InternalServisError>();
            }
            
            return result;
        }
        #endregion 
        
    }
}
