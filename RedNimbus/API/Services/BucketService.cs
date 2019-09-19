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
        public Either<IError, List<string>> Get(string token, Guid requestId)
        {
            Message<BucketMessage> message = new Message<BucketMessage>("bucket/listBucketContent")
            {
                Data = new BucketMessage()
                {
                    Token = token,
                    Path = "/"
                }
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage(), requestId);

            string responseTopic = response.First.ConvertToString();
             
            if (responseTopic.Equals("Response"))
            {
                Message<BucketMessage> successMessage = new Message<BucketMessage>(response);

                List<string> bucketContent = new List<string>(successMessage.Data.ReturnItems);

                return new Right<IError, List<string>>(bucketContent);
            }

            return new Left<IError, List<string>>(GetError(response));
        }

        public Either<IError, List<string>> ListBucketContent(string token, string id, Guid requestId)
        {
            Message<BucketMessage> message = new Message<BucketMessage>("bucket/listBucketContent")
            {
                Data = new BucketMessage()
                {
                    Token = token,
                    Path = "/" + id
                }
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage(), requestId);

            string responseTopic = response.First.ConvertToString();

            if (responseTopic.Equals("Response"))
            {
                Message<BucketMessage> successMessage = new Message<BucketMessage>(response);

                List<string> bucketContent = new List<string>(successMessage.Data.ReturnItems);

                return new Right<IError, List<string>>(bucketContent);
            }

            return new Left<IError, List<string>>(GetError(response));
        }

        public Either<IError, StringDto> CreateBucket(string token, StringDto bucketName, Guid requestId)
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

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage(), requestId);

            string responseTopic = response.First.ConvertToString();


            if (responseTopic.Equals("Response"))
            {
                Message<BucketMessage> successMessage = new Message<BucketMessage>(response);
                bucketName.Value = successMessage.Data.ReturnItems[0];
                return new Right<IError, StringDto>(bucketName);
            }

            return new Left<IError, StringDto>(GetError(response));
        }

        public Either<IError, StringDto> DeleteBucket(string token, StringDto bucketName, Guid requestId)
        {
            Message<BucketMessage> message = new Message<BucketMessage>("bucket/deleteBucket")
            {
                Data = new BucketMessage()
                {
                    Token = token,
                    Path = bucketName.Path+ "/" + bucketName.Value
                }
            };

            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage(), requestId);

            string responseTopic = response.First.ConvertToString();

            if (responseTopic.Equals("Response"))
            {
                 return new Right<IError, StringDto>(bucketName);
            }

            return new Left<IError, StringDto>(GetError(response));
        }
        
        public Either<IError, UploadFileDto> UploadFile(string token, UploadFileDto uploadFile, Guid requestId)
        {
            if(uploadFile.Path.Equals("/"))
                return new Left<IError, UploadFileDto>(new FormatError("You must be in bucket to upload file.", Either.Enums.ErrorCode.PutFileError));
            Message<BucketMessage> message;

            using (Stream stream = new MemoryStream())
            {
                uploadFile.File.CopyTo(stream);
                message = new Message<BucketMessage>("bucket/putFile")
                {
                    Data = new BucketMessage()
                    {
                        Token = token,
                        Path = uploadFile.Path + uploadFile.Value
                    },
                    Bytes = new NetMQFrame(stream.ToByteArray())
                };
            }
            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage(), requestId);
            
            string responseTopic = response.First.ConvertToString();


            if (responseTopic.Equals("Response"))
            {
                return new Right<IError, UploadFileDto>(uploadFile);
            }

            return new Left<IError, UploadFileDto>(GetError(response));

        }

        public Either<IError, StringDto> DeleteFile(string token, StringDto fileName, Guid requestId)
        {
            Message<BucketMessage> message = new Message<BucketMessage>("bucket/deleteFile")
            {
                Data = new BucketMessage()
                {
                    Token = token,
                    Path = fileName.Path + "/" + fileName.Value
                }
            };


            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage(), requestId);

            string responseTopic = response.First.ConvertToString();


            if (responseTopic.Equals("Response"))
            {
                 return new Right<IError, StringDto>(fileName);
            }

            return new Left<IError, StringDto>(GetError(response));

        }

        public Either<IError, DownloadFileDto> DownloadFile(string token, StringDto fileName, Guid requestId)
        {
            Message<BucketMessage> message = new Message<BucketMessage>("bucket/getFile")
            {
                Data = new BucketMessage()
                {
                    Token = token,
                    Path = fileName.Path + "/" + fileName.Value
                }
            };


            NetMQMessage response = RequestSocketFactory.SendRequest(message.ToNetMQMessage(), requestId);

            string responseTopic = response.First.ConvertToString();


            if (responseTopic.Equals("Response"))
            {
                Message<BucketMessage> successMessage = new Message<BucketMessage>(response);
                byte[] data = successMessage.Bytes.ToByteArray();
                DownloadFileDto upload = new DownloadFileDto();
                upload.File = data;
                upload.Value = fileName.Value;
                upload.Type = GetContentType(fileName.Value);
                return new Right<IError, DownloadFileDto>(upload);
            }

            return new Left<IError, DownloadFileDto>(GetError(response));

        }

        
        private static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            if(types.ContainsKey(ext))
                return types[ext];
            return "text/plain";
        }


        private static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats  officedocument.spreadsheetml.sheet"},  
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".exe", "application/vnd.microsoft.portable-executable"},
                {".xsd", "application/xml"},
                {".rar", "application/x-rar-compressed"},
                {".zip", "application/zip"}
            };
        }

    }
}

