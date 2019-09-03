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

        }

        public void CreateBucket(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg);

            if (FileSystemService.NumberOfDirectories(HomePath(msg.Data.Token)) < 5)
                msg.Data.Successful = FileSystemService.CreateFolder(absolutePath);
            else
                msg.Data.Successful = false;

            msg.Topic = _returnTopic;

            if (!msg.Data.Successful)
            {
                msg.Data.ErrorMessage = "Error";
            }

            SendMessage(msg.ToNetMQMessage());
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

            }
            else
            {
                msg.Data.ErrorMessage = "Error";
            }
            
            SendMessage(msg.ToNetMQMessage());
        }

        public void CreateFolder(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg);

            msg.Data.Successful = FileSystemService.CreateFolder(absolutePath);
            msg.Topic = _returnTopic;

            if (!msg.Data.Successful)
            { 
                msg.Data.ErrorMessage = "Error";
            }

            SendMessage(msg.ToNetMQMessage());
        }

        //Bucket must be empty for deleting
        public void DeleteFolder(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg);

            msg.Data.Successful = FileSystemService.DeleteFolder(absolutePath);
            msg.Topic = _returnTopic; 

            if (!msg.Data.Successful)
            {
                msg.Data.ErrorMessage = "Error";
            }
            
            SendMessage(msg.ToNetMQMessage());
        }

        public void PutFile(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg);
            byte[] fileAsByteArray = msg.Data.File.ToByteArray();

            msg.Data.Successful = FileSystemService.ByteArrayToFile(absolutePath, fileAsByteArray);
            msg.Topic = _returnTopic;

            if (!msg.Data.Successful)
            {
                msg.Data.ErrorMessage = "Error";
            }

            SendMessage(msg.ToNetMQMessage());
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
            }
            else
            {
                msg.Data.Successful = false;
                msg.Data.ErrorMessage = "Error";
            }

            SendMessage(msg.ToNetMQMessage());
        }

        public void DeleteFile(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string absolutePath = MessageHelper.GetAbsolutePath(_path, msg);

            msg.Data.Successful = FileSystemService.DeleteFile(absolutePath);
            msg.Topic = _returnTopic;

            if (!msg.Data.Successful)
            {
                msg.Data.ErrorMessage = "Error";
            }
            
            SendMessage(msg.ToNetMQMessage());
        }

        public string HomePath(string token)
        {
            return _path + MessageHelper.Decode(token);
        }
    }
}
