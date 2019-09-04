using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetMQ;
using NetMQ.Sockets;
using RedNimbus.API.Controllers;
using RedNimbus.API.Helper;
using RedNimbus.API.Services;
using RedNimbus.Communication;
using RedNimbus.DTO;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using RedNimbus.Messages;

namespace RedNimbus.API.Services
{
    public class BucketService : BaseService
    {

        //Get user Buckets
        public Either<IError, List<string>> Get(string token)
        {
            Message<BucketMessage> message = new Message<BucketMessage>("bucket/listBucketContent")
            {
                Data = new BucketMessage()
                {
                    Token = token,
                    Path = "/"
                }
            };


            NetMQMessage temp = message.ToNetMQMessage();
            NetMQFrame topicFrame = temp.Pop();
            NetMQFrame emptyFrame = temp.Pop();
            temp.Push(topicFrame);

            NetMQMessage response = RequestSocketFactory.SendRequest(temp);

            string responseTopic = response.First.ConvertToString();


            if (responseTopic.Equals("Response"))
            {
                Message<BucketMessage> successMessage = new Message<BucketMessage>(response);

                List<string> bucketContent = new List<string>(successMessage.Data.ReturnItems);

                return new Right<IError, List<string>>(bucketContent);
            }

            return new Left<IError, List<string>>(GetError(response));
        }

        public Either<IError, List<string>> ListBucketContent(string token, string id)
        {
            Message<BucketMessage> message = new Message<BucketMessage>("bucket/listBucketContent")
            {
                Data = new BucketMessage()
                {
                    Token = token,
                    Path = "/" + id
                }
            };


            NetMQMessage temp = message.ToNetMQMessage();
            NetMQFrame topicFrame = temp.Pop();
            NetMQFrame emptyFrame = temp.Pop();
            temp.Push(topicFrame);

            NetMQMessage response = RequestSocketFactory.SendRequest(temp);

            string responseTopic = response.First.ConvertToString();


            if (responseTopic.Equals("Response"))
            {
                Message<BucketMessage> successMessage = new Message<BucketMessage>(response);

                List<string> bucketContent = new List<string>(successMessage.Data.ReturnItems);

                return new Right<IError, List<string>>(bucketContent);
            }

            return new Left<IError, List<string>>(GetError(response));
        }

        public Either<IError, StringDto> CreateBucket(string token, StringDto bucketName)
        {
            Message<BucketMessage> message = new Message<BucketMessage>("bucket/createBucket")
            {
                Data = new BucketMessage()
                {
                    Token = token,
                    Path = "/" + bucketName.Value
                }
            };


            NetMQMessage temp = message.ToNetMQMessage();
            NetMQFrame topicFrame = temp.Pop();
            NetMQFrame emptyFrame = temp.Pop();
            temp.Push(topicFrame);

            NetMQMessage response = RequestSocketFactory.SendRequest(temp);

            string responseTopic = response.First.ConvertToString();


            if (responseTopic.Equals("Response"))
            {
                Message<BucketMessage> successMessage = new Message<BucketMessage>(response);

                return new Right<IError, StringDto>(bucketName);
            }

            return new Left<IError, StringDto>(GetError(response));

        }

        public Either<IError, bool> DeleteBucket(string token, StringDto bucketName)
        {
            Message<BucketMessage> message = new Message<BucketMessage>("bucket/deleteBucket")
            {
                Data = new BucketMessage()
                {
                    Token = token,
                    Path = "/" + bucketName.Value
                }
            };


            NetMQMessage temp = message.ToNetMQMessage();
            NetMQFrame topicFrame = temp.Pop();
            NetMQFrame emptyFrame = temp.Pop();
            temp.Push(topicFrame);

            NetMQMessage response = RequestSocketFactory.SendRequest(temp);

            string responseTopic = response.First.ConvertToString();


            if (responseTopic.Equals("Response"))
            {
                Message<BucketMessage> successMessage = new Message<BucketMessage>(response);

                return new Right<IError, bool>(true);
            }

            return new Left<IError, bool>(GetError(response));

        }
    }
}
