using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DTO;
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
            Message<BucketMessage> message;
            if (bucketName.Path.Equals("/"))
            {
                message = new Message<BucketMessage>("bucket/createBucket")
                {
                    Data = new BucketMessage()
                    {
                        Token = token,
                        Path = bucketName.Path + bucketName.Value
                    }
                };
            }
            else
            {
                message = new Message<BucketMessage>("bucket/createFolder")
                {
                    Data = new BucketMessage()
                    {
                        Token = token,
                        Path = bucketName.Path + "/" + bucketName.Value
                    }
                };
            }


            NetMQMessage temp = message.ToNetMQMessage();
            NetMQFrame topicFrame = temp.Pop();
            NetMQFrame emptyFrame = temp.Pop();
            temp.Push(topicFrame);

            NetMQMessage response = RequestSocketFactory.SendRequest(temp);

            string responseTopic = response.First.ConvertToString();


            if (responseTopic.Equals("Response"))
            {
                Message<BucketMessage> successMessage = new Message<BucketMessage>(response);
                bucketName.Value = successMessage.Data.ReturnItems[0];
                return new Right<IError, StringDto>(bucketName);
            }

            return new Left<IError, StringDto>(GetError(response));

        }

        public Either<IError, StringDto> DeleteBucket(string token, StringDto bucketName)
        {
            Message<BucketMessage> message = new Message<BucketMessage>("bucket/deleteBucket")
            {
                Data = new BucketMessage()
                {
                    Token = token,
                    Path = bucketName.Path+ "/" + bucketName.Value
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
        
        public Either<IError, UploadFileDto> UploadFile(string token, UploadFileDto uploadFile)
        {
            string[] decodeHelp = uploadFile.File.Split(",");
            if(uploadFile.Path.Equals("/"))
                return new Left<IError, UploadFileDto>(new FormatError("You must be in bucket to upload file.", Either.Enums.ErrorCode.PutFileError));

            Message<BucketMessage> message = new Message<BucketMessage>("bucket/putFile")
            {
                Data = new BucketMessage()
                {
                    Token = token,
                    Path = uploadFile.Path + uploadFile.Value,
                    File = ByteString.CopyFrom(System.Convert.FromBase64String(decodeHelp[1]))
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

                return new Right<IError, UploadFileDto>(uploadFile);
            }

            return new Left<IError, UploadFileDto>(GetError(response));

        }

        public Either<IError, StringDto> DeleteFile(string token, StringDto fileName)
        {
            Message<BucketMessage> message = new Message<BucketMessage>("bucket/deleteFile")
            {
                Data = new BucketMessage()
                {
                    Token = token,
                    Path = fileName.Path + "/" + fileName.Value
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

                return new Right<IError, StringDto>(fileName);
            }

            return new Left<IError, StringDto>(GetError(response));

        }

        public Either<IError, StringDto> DownloadFile(string token, StringDto fileName)
        {
            Message<BucketMessage> message = new Message<BucketMessage>("bucket/getFile")
            {
                Data = new BucketMessage()
                {
                    Token = token,
                    Path = fileName.Path + "/" + fileName.Value
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
                byte[] data = successMessage.Data.File.ToByteArray();
                string base64Str = Convert.ToBase64String(data);
                fileName.Value = Translate(fileName.Value)+ ";base64," + base64Str;
                return new Right<IError, StringDto>(fileName);
            }

            return new Left<IError, StringDto>(GetError(response));

        }

        public static string Translate(string name)
        {
            string extension = Path.GetExtension(name);

            return "data:text/plain";
        }

    }
}

