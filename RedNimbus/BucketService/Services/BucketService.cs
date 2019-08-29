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

            Subscribe("bucket/listBucketContent", ListBucketContent);
            Subscribe("bucket/createBucket", CreateBucket);
            Subscribe("bucket/deleteBucket", DeleteBucket);
            Subscribe("bucket/putFile", PutFile);
            Subscribe("bucket/getFile", GetFile);
            Subscribe("bucket/deleteFile", DeleteFile);
        }

        public void ListBucketContent(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);

            string relativePath = msg.Data.Path;
            string userGuid = MessageHelper.Decode(msg.Data.Token) ;
            string absolutePath = _path + userGuid + relativePath;

            List<string> contentList = FileSystemService.ListContent(absolutePath);

            msg.Data.ReturnItems.AddRange(contentList);
            msg.Topic = "Response";
            msg.Data.Successful = true;
            SendMessage(msg.ToNetMQMessage());
        }

        public void CreateBucket(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string relativePath = msg.Data.Path;
            string userGuid = MessageHelper.Decode(msg.Data.Token);
            string absolutePath = _path + userGuid + relativePath;

            bool successfulCreate = FileSystemService.CreateBucket(absolutePath);
            msg.Topic = "Response";
            if (successfulCreate)
            {
                msg.Data.Successful=true;
                
                SendMessage(msg.ToNetMQMessage());
            }
            else
            {
                msg.Data.Successful = true;
                msg.Data.ErrorMessage = "Ugh";
                SendMessage(msg.ToNetMQMessage());
            }
        }

        //Bucket must be empty for deleting
        public void DeleteBucket(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string relativePath = msg.Data.Path;
            string userGuid = MessageHelper.Decode(msg.Data.Token);
            string absolutePath = _path + userGuid + relativePath;

            bool successfulDelete = FileSystemService.DeleteBucket(absolutePath);
            msg.Topic = "Response";
            if (successfulDelete)
            {
                msg.Data.Successful = true;

                SendMessage(msg.ToNetMQMessage());
            }
            else
            {
                msg.Data.Successful = true;
                msg.Data.ErrorMessage = "Ugh";
                SendMessage(msg.ToNetMQMessage());
            }
        }

        public void PutFile(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string relativePath = msg.Data.Path;
            string userGuid = MessageHelper.Decode(msg.Data.Token);
            string absolutePath = _path + userGuid + relativePath;
            byte[] fileAsByteArray = msg.Data.File.ToByteArray();

            FileSystemService.ByteArrayToFile(absolutePath, fileAsByteArray);
            msg.Topic = "Response";
            msg.Data.Successful = true;

            SendMessage(msg.ToNetMQMessage());
        }

        public void GetFile(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string relativePath = msg.Data.Path;
            string userGuid = MessageHelper.Decode(msg.Data.Token);
            string absolutePath = _path + userGuid + relativePath;

            byte[] fileAsByteArray = FileSystemService.FileToByteArray(absolutePath);
            msg.Topic = "Response";
            msg.Data.File = ByteString.CopyFrom(fileAsByteArray);
            SendMessage(msg.ToNetMQMessage());
        }

        public void DeleteFile(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string relativePath = msg.Data.Path;
            string userGuid = MessageHelper.Decode(msg.Data.Token); ;
            string absolutePath = _path + userGuid + relativePath;

            FileSystemService.DeleteFile(absolutePath);
            msg.Topic = "Response";
            msg.Data.Successful = true;
            SendMessage(msg.ToNetMQMessage());
        }
    }
}
