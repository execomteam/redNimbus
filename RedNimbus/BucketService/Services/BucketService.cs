using System;
using System.Collections.Generic;
using RedNimbus.BucketService.Helper;
using NetMQ;
using RedNimbus.Communication;
using RedNimbus.Messages;
using RedNimbus.TokenManager;
using System.IO;
using RedNimbus.LogLibrary;
using Google.Protobuf;

namespace RedNimbus.BucketService.Services
{
    public class BucketService : BaseService
    {
        private string _path;
        private int _maxFileSize;
        private ITokenManager _tokenManager;
        private ILogSender _logSender;

        public BucketService(string path, ITokenManager manager, int maxFileSize, ILogSender logSender) : base()
        {
            _path = path;
            _tokenManager = manager;
            _maxFileSize = maxFileSize;
            _logSender = logSender;

            Subscribe("bucket/createBucket", CreateBucket);
            Subscribe("bucket/deleteBucket", DeleteBucket);
            Subscribe("bucket/listBucketContent", ListBucketContent);
            Subscribe("bucket/createFolder", CreateFolder);
            Subscribe("bucket/deleteFolder", DeleteFolder);
            Subscribe("bucket/putFile", PutFile);
            Subscribe("bucket/getFile", GetFile);
            Subscribe("bucket/deleteFile", DeleteFile);
        }

        private void LogBucketMessage(NetMQMessage message, string origin, LogMessage.Types.LogType type)
        {
            BucketMessage payload = new BucketMessage();
            payload.MergeFrom(message[2].ToByteArray());
            LogMessage logMessage = new LogMessage()
            {
                Origin = origin,
                Payload = String.Format("token = {0}, path: {1}, received bytes: {2}, returned items: {3}, returned bytes: {4}", payload.Token, payload.Path, payload.File.ToByteArray().Length, payload.ReturnItems, message[3].ToByteArray().Length),
                Date = DateTime.Now.ToShortDateString(),
                Time = DateTime.Now.TimeOfDay.ToString(),
                Type = type
            };

            _logSender.Send(new Guid(message[1].ToByteArray()), logMessage);
        }

        public void DeleteBucket(NetMQMessage message)
        {
            LogBucketMessage(message, "BucketService/DeleteBucket - message received from event bus", LogMessage.Types.LogType.Info);
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg, _tokenManager);


            if (absolutePath == null)
            {
                msg.Data.Successful = false;
                SendErrorMessage("Delete bucket error", Either.Enums.ErrorCode.DeleteFolderError, msg.Id);
                LogBucketMessage(message, "BucketService/DeleteBucket - invalid path", LogMessage.Types.LogType.Info);
            }
            else
            {
                msg.Data.Successful = FileSystemService.DeleteFolder(absolutePath);
                msg.Topic = _returnTopic;
                SendMessage(msg.ToNetMQMessage());
                LogBucketMessage(msg.ToNetMQMessage(), "BucketService/DeleteBucket - bucket successfuly deleted", LogMessage.Types.LogType.Info);
            }
        }

        public void CreateBucket(NetMQMessage message)
        {
            LogBucketMessage(message, "BucketService/CreateBucket - message received from event bus", LogMessage.Types.LogType.Info);
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg, _tokenManager);
            string bucketName = null;
            
            if (absolutePath == null)
            {
                msg.Data.Successful = false;
                SendErrorMessage("Create bucket error", Either.Enums.ErrorCode.CreateBucketError, msg.Id);
                LogBucketMessage(message, "BucketService/CreateBucket - invalid path", LogMessage.Types.LogType.Info);
            }
            else
            {
                if (!Directory.Exists(HomePath(msg.Data.Token)))
                {
                    FileSystemService.CreateFolder(HomePath(msg.Data.Token));
                }

                if (FileSystemService.NumberOfDirectories(HomePath(msg.Data.Token)) < 5)
                {
                    bucketName = FileSystemService.CreateFolder(absolutePath);
                    msg.Data.Successful = bucketName != null;
                    msg.Topic = _returnTopic;
                    msg.Data.ReturnItems.Add(bucketName);
                    SendMessage(msg.ToNetMQMessage());
                    LogBucketMessage(msg.ToNetMQMessage(), "BucketService/CreateBucket - bucket successfuly created", LogMessage.Types.LogType.Info);
                }
                else
                {
                    SendErrorMessage("Maximum number of buckets is 5", Either.Enums.ErrorCode.NumberOfBucketsExeeded, msg.Id);
                    LogBucketMessage(message, "BucketService/CreateBucket - bucket limit reached", LogMessage.Types.LogType.Info);
                    return;
                }
            } 
        }

        public void ListBucketContent(NetMQMessage message)
        {
            LogBucketMessage(message, "BucketService/ListBucketContent - message received", LogMessage.Types.LogType.Info);

            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg, _tokenManager);
            List<string> contentList;

            if (absolutePath == null)
            {
                contentList = null;
                SendErrorMessage("List bucket content error", Either.Enums.ErrorCode.ListBucketContentError, msg.Id);
                LogBucketMessage(msg.ToNetMQMessage(), "BucketService/ListBucketContent - invalid path", LogMessage.Types.LogType.Info);
            }
            else
            {
                if (!Directory.Exists(HomePath(msg.Data.Token)))
                {
                    FileSystemService.CreateFolder(HomePath(msg.Data.Token));
                }
                contentList = FileSystemService.ListContent(absolutePath);

                msg.Data.Successful = true;
                msg.Topic = _returnTopic;
                msg.Data.ReturnItems.AddRange(contentList);
                SendMessage(msg.ToNetMQMessage());
                LogBucketMessage(msg.ToNetMQMessage(), "BucketService/ListBucketContent - bucket listing successful", LogMessage.Types.LogType.Info);
            }
        }

        public void CreateFolder(NetMQMessage message)
        {
            LogBucketMessage(message, "BucketService/CreateFolder - message received from event bus", LogMessage.Types.LogType.Info);

            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg, _tokenManager);
            string folderName = null;
            if (absolutePath == null)
            {
                msg.Data.Successful = false;
                SendErrorMessage("Create folder error", Either.Enums.ErrorCode.CreateFolderError, msg.Id);
                LogBucketMessage(message, "BucketService/CreateFolder - invalid path", LogMessage.Types.LogType.Info);
            }
            else
            {
                if (!Directory.Exists(HomePath(msg.Data.Token)))
                {
                    FileSystemService.CreateFolder(HomePath(msg.Data.Token));
                }

                folderName = FileSystemService.CreateFolder(absolutePath);

                msg.Data.Successful = true;
                msg.Topic = _returnTopic;
                msg.Data.ReturnItems.Add(folderName);
                SendMessage(msg.ToNetMQMessage());
                LogBucketMessage(msg.ToNetMQMessage(), "BucketService/CreateFolder - bucket created successfuly", LogMessage.Types.LogType.Info);
            }
        }

        //Bucket must be empty for deleting
        public void DeleteFolder(NetMQMessage message)
        {
            LogBucketMessage(message, "BucketService/DeleteFolder - message received from event bus", LogMessage.Types.LogType.Info);

            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg, _tokenManager);

            if (absolutePath == null)
            {
                SendErrorMessage("Delete folder error", Either.Enums.ErrorCode.DeleteFolderError, msg.Id);
                LogBucketMessage(message, "BucketService/DeleteFolder - invalid path", LogMessage.Types.LogType.Info);
            }
            else
            {
                msg.Data.Successful = true;
                FileSystemService.DeleteFolder(absolutePath);
                msg.Topic = _returnTopic;
                SendMessage(msg.ToNetMQMessage());
                LogBucketMessage(msg.ToNetMQMessage(), "BucketService/DeleteFolder - file deleted successfuly", LogMessage.Types.LogType.Info);
            }
        }

        public void PutFile(NetMQMessage message)
        {
            LogBucketMessage(message, "BucketService/PutFile - message received from event bus", LogMessage.Types.LogType.Info);
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg, _tokenManager);
            byte[] fileAsByteArray = msg.Bytes.ToByteArray();

            if (absolutePath == null)
            {
                msg.Data.Successful = false;
                SendErrorMessage("Put file error", Either.Enums.ErrorCode.PutFileError, msg.Id);
                LogBucketMessage(msg.ToNetMQMessage(), "BucketService/PutFile - invalid path", LogMessage.Types.LogType.Info);
            }
            else
            {
                if (!Directory.Exists(HomePath(msg.Data.Token)))
                {
                    FileSystemService.CreateFolder(HomePath(msg.Data.Token));
                }
                if (fileAsByteArray.Length > _maxFileSize)
                {
                    SendErrorMessage("File size limit exceed.", Either.Enums.ErrorCode.PutFileError, msg.Id);
                    LogBucketMessage(message, "BucketService/PutFile - max file size limit", LogMessage.Types.LogType.Info);
                    return;
                }

                msg.Data.Successful = FileSystemService.ByteArrayToFile(absolutePath, fileAsByteArray);
                msg.Topic = _returnTopic;
                SendMessage(msg.ToNetMQMessage());
                LogBucketMessage(msg.ToNetMQMessage(), "BucketService/PutFile - put file successful", LogMessage.Types.LogType.Info);
            }
        }

        public void GetFile(NetMQMessage message)
        {
            LogBucketMessage(message, "BucketService/GetFile - message received from event bus", LogMessage.Types.LogType.Info);

            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg, _tokenManager);
            byte[] fileAsByteArray;

            if (absolutePath == null)
            {
                SendErrorMessage("Get file error", Either.Enums.ErrorCode.GetFileError, msg.Id);
                LogBucketMessage(message, "BucketService/GetFile - invalid path", LogMessage.Types.LogType.Info);
            }
            else
            {
                if (!Directory.Exists(HomePath(msg.Data.Token)))
                {
                    FileSystemService.CreateFolder(HomePath(msg.Data.Token));
                }

                fileAsByteArray = FileSystemService.FileToByteArray(absolutePath);
                msg.Topic = _returnTopic;
                msg.Data.Successful = true;
                msg.Bytes = new NetMQFrame(fileAsByteArray);
                SendMessage(msg.ToNetMQMessage());
                LogBucketMessage(msg.ToNetMQMessage(), "BucketService/GetFile - get file successful", LogMessage.Types.LogType.Info);
            }
        }

        public void DeleteFile(NetMQMessage message)
        {
            LogBucketMessage(message, "BucketService/DeleteFile - message received from event bus", LogMessage.Types.LogType.Info);

            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg, _tokenManager);

            if (absolutePath == null)
            {
                SendErrorMessage("Delete file error", Either.Enums.ErrorCode.DeleteFileError, msg.Id);
                LogBucketMessage(message, "BucketService/DeleteFile - invalid path", LogMessage.Types.LogType.Info);
            }
            else
            {
                msg.Data.Successful = FileSystemService.DeleteFile(absolutePath);
                msg.Topic = _returnTopic;
                SendMessage(msg.ToNetMQMessage());
                LogBucketMessage(msg.ToNetMQMessage(), "BucketService/DeleteFile - file deleted successfuly", LogMessage.Types.LogType.Info);
            }
        }

        private string HomePath(string token)
        {
            Guid guid = _tokenManager.ValidateToken(token);
            return _path + guid.ToString("B");
        }
    }
}
