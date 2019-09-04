using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RedNimbus.BucketService.Helper;
using NetMQ;
using RedNimbus.Communication;
using RedNimbus.BucketService.Services;
using RedNimbus.Messages;
using Google.Protobuf;

namespace RedNimbus.BucketService.Services
{
    public class BucketService : BaseService
    {
        private string _path;
        

        public BucketService(string path) : base()
        {
            _path = path;

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
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg);

            msg.Data.Successful = FileSystemService.DeleteFolder(absolutePath);
            msg.Topic = _returnTopic;

            if (msg.Data.Successful)
            {
                SendMessage(msg.ToNetMQMessage());
            }
            else
            {
                SendErrorMessage("Delete bucket error", Either.Enums.ErrorCode.DeleteFolderError, msg.Id);
            }
        }

        public void CreateBucket(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg);

            if (FileSystemService.NumberOfDirectories(HomePath(msg.Data.Token)) < 5)
                msg.Data.Successful = FileSystemService.CreateFolder(absolutePath);
            else
            {
                SendErrorMessage("Maximum number of buckets is 5", Either.Enums.ErrorCode.NumberOfBucketsExeeded, msg.Id);
                msg.Data.Successful = false;
            }
            
            if (msg.Data.Successful)
            {
                msg.Topic = _returnTopic;
                SendMessage(msg.ToNetMQMessage());
            }
            else
            {
                SendErrorMessage("Create bucket error", Either.Enums.ErrorCode.CreateBucketError, msg.Id);
            }
                
        }

        public void ListBucketContent(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg);

            List<string> contentList = FileSystemService.ListContent(absolutePath);
            msg.Data.Successful = (contentList != null);
            msg.Topic = _returnTopic;

            if (msg.Data.Successful)
            {
                msg.Data.ReturnItems.AddRange(contentList);
                SendMessage(msg.ToNetMQMessage());
            }
            else
            {
                SendErrorMessage("List bucket content error", Either.Enums.ErrorCode.ListBucketContentError, msg.Id);
            }
        }

        public void CreateFolder(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg);

            msg.Data.Successful = FileSystemService.CreateFolder(absolutePath);
            msg.Topic = _returnTopic;

            if (msg.Data.Successful)
            {
                SendMessage(msg.ToNetMQMessage());
            }
            else
            {
                SendErrorMessage("Create folder error", Either.Enums.ErrorCode.CreateFolderError, msg.Id);
            }
        }

        //Bucket must be empty for deleting
        public void DeleteFolder(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg);

            msg.Data.Successful = FileSystemService.DeleteFolder(absolutePath);
            msg.Topic = _returnTopic; 

            if (msg.Data.Successful)
            {
                SendMessage(msg.ToNetMQMessage());
            }
            else
            {
                SendErrorMessage("Delete folder error", Either.Enums.ErrorCode.DeleteFolderError, msg.Id);
            }
        }

        public void PutFile(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg);
            byte[] fileAsByteArray = msg.Data.File.ToByteArray();

            msg.Data.Successful = FileSystemService.ByteArrayToFile(absolutePath, fileAsByteArray);
            msg.Topic = _returnTopic;

            if (msg.Data.Successful)
            {
                SendMessage(msg.ToNetMQMessage());
            }
            else
            {
                SendErrorMessage("Put file error", Either.Enums.ErrorCode.PutFileError, msg.Id);
            }
        }

        public void GetFile(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg);

            byte[] fileAsByteArray = FileSystemService.FileToByteArray(absolutePath);
            msg.Topic = _returnTopic;

            if (fileAsByteArray != null)
            {
                msg.Data.Successful = true;
                msg.Data.File = ByteString.CopyFrom(fileAsByteArray);
                SendMessage(msg.ToNetMQMessage());
            }
            else
            {
                SendErrorMessage("Get file error", Either.Enums.ErrorCode.GetFileError, msg.Id);
            }
        }

        public void DeleteFile(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg);

            msg.Data.Successful = FileSystemService.DeleteFile(absolutePath);
            msg.Topic = _returnTopic;

            if (msg.Data.Successful)
            {
                SendMessage(msg.ToNetMQMessage());
            }
            else
            {
                SendErrorMessage("Delete file error", Either.Enums.ErrorCode.DeleteFileError, msg.Id);
            }
        }

        private string HomePath(string token)
        {
            return _path + MessageHelper.Decode(token);
        }
    }
}
