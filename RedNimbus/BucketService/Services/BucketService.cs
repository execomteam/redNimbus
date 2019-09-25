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

        public BucketService(string path, ITokenManager manager, int maxFileSize) : base()
        {
            _path = path;
            _tokenManager = manager;
            _maxFileSize = maxFileSize;

            Subscribe("bucket/createBucket", CreateBucket);
            Subscribe("bucket/deleteBucket", DeleteBucket);
            Subscribe("bucket/listBucketContent", ListBucketContent);
            Subscribe("bucket/createFolder", CreateFolder);
            Subscribe("bucket/deleteFolder", DeleteFolder);
            Subscribe("bucket/putFile", PutFile);
            Subscribe("bucket/getFile", GetFile);
            Subscribe("bucket/deleteFile", DeleteFile);
        }

        public void DeleteBucket(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg, _tokenManager);


            if (absolutePath == null)
            {
                msg.Data.Successful = false;
                SendErrorMessage("Delete bucket error", Either.Enums.ErrorCode.DeleteFolderError, msg.Id);
            }
            else
            {
                msg.Data.Successful = FileSystemService.DeleteFolder(absolutePath);
                msg.Topic = _returnTopic;
                SendMessage(msg.ToNetMQMessage());
            }
        }

        public void CreateBucket(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg, _tokenManager);
            string bucketName = null;
            
            if (absolutePath == null)
            {
                msg.Data.Successful = false;
                SendErrorMessage("Create bucket error", Either.Enums.ErrorCode.CreateBucketError, msg.Id);
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
                }
                else
                {
                    SendErrorMessage("Maximum number of buckets is 5", Either.Enums.ErrorCode.NumberOfBucketsExeeded, msg.Id);
                    return;
                }
            } 
        }

        public void ListBucketContent(NetMQMessage message)
        {

            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg, _tokenManager);
            List<string> contentList;

            if (absolutePath == null)
            {
                contentList = null;
                SendErrorMessage("List bucket content error", Either.Enums.ErrorCode.ListBucketContentError, msg.Id);
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
            }
        }

        public void CreateFolder(NetMQMessage message)
        {

            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg, _tokenManager);
            string folderName = null;
            if (absolutePath == null)
            {
                msg.Data.Successful = false;
                SendErrorMessage("Create folder error", Either.Enums.ErrorCode.CreateFolderError, msg.Id);
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
            }
        }

        //Bucket must be empty for deleting
        public void DeleteFolder(NetMQMessage message)
        {

            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg, _tokenManager);

            if (absolutePath == null)
            {
                SendErrorMessage("Delete folder error", Either.Enums.ErrorCode.DeleteFolderError, msg.Id);
            }
            else
            {
                msg.Data.Successful = true;
                FileSystemService.DeleteFolder(absolutePath);
                msg.Topic = _returnTopic;
                SendMessage(msg.ToNetMQMessage());
            }
        }

        public void PutFile(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg, _tokenManager);
            byte[] fileAsByteArray = msg.Bytes.ToByteArray();

            if (absolutePath == null)
            {
                msg.Data.Successful = false;
                SendErrorMessage("Put file error", Either.Enums.ErrorCode.PutFileError, msg.Id);
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
                    return;
                }

                msg.Data.Successful = FileSystemService.ByteArrayToFile(absolutePath, fileAsByteArray);
                msg.Topic = _returnTopic;
                SendMessage(msg.ToNetMQMessage());
            }
        }

        public void GetFile(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg, _tokenManager);
            byte[] fileAsByteArray;

            if (absolutePath == null)
            {
                SendErrorMessage("Get file error", Either.Enums.ErrorCode.GetFileError, msg.Id);
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
            }
        }

        public void DeleteFile(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg, _tokenManager);

            if (absolutePath == null)
            {
                SendErrorMessage("Delete file error", Either.Enums.ErrorCode.DeleteFileError, msg.Id);
            }
            else
            {
                msg.Data.Successful = FileSystemService.DeleteFile(absolutePath);
                msg.Topic = _returnTopic;
                SendMessage(msg.ToNetMQMessage());
            }
        }

        private string HomePath(string token)
        {
            Guid guid = _tokenManager.ValidateToken(token);
            return _path + guid.ToString("B");
        }
    }
}
