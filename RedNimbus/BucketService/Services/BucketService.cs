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
            msg.Data.Successful = (contentList != null);
            msg.Topic = "Response";

            if (msg.Data.Successful)
            {
                msg.Data.ReturnItems.AddRange(contentList);

            }
            else
            {
                msg.Data.ErrorMessage = "Ugh";
            }
            
            SendMessage(msg.ToNetMQMessage());
        }

        public void CreateBucket(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string relativePath = msg.Data.Path;
            string userGuid = MessageHelper.Decode(msg.Data.Token);
            string absolutePath = _path + userGuid + relativePath;

            msg.Data.Successful = FileSystemService.CreateBucket(absolutePath);
            msg.Topic = "Response";

            if (!msg.Data.Successful)
            { 
                msg.Data.ErrorMessage = "Ugh";
            }

            SendMessage(msg.ToNetMQMessage());
        }

        //Bucket must be empty for deleting
        public void DeleteBucket(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string relativePath = msg.Data.Path;
            string userGuid = MessageHelper.Decode(msg.Data.Token);
            string absolutePath = _path + userGuid + relativePath;

            msg.Data.Successful = FileSystemService.DeleteBucket(absolutePath);
            msg.Topic = "Response";

            if (!msg.Data.Successful)
            {
                msg.Data.ErrorMessage = "Ugh";
            }
            
            SendMessage(msg.ToNetMQMessage());
        }

        public void PutFile(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string relativePath = msg.Data.Path;
            string userGuid = MessageHelper.Decode(msg.Data.Token);
            string absolutePath = _path + userGuid + relativePath;
            byte[] fileAsByteArray = msg.Data.File.ToByteArray();

            msg.Data.Successful = FileSystemService.ByteArrayToFile(absolutePath, fileAsByteArray);
            msg.Topic = "Response";

            if (!msg.Data.Successful)
            {
                msg.Data.ErrorMessage = "Ugh";
            }

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

            if (fileAsByteArray != null)
            {
                msg.Data.Successful = true;
                msg.Data.File = ByteString.CopyFrom(fileAsByteArray);
            }
            else
            {
                msg.Data.Successful = false;
                msg.Data.ErrorMessage = "Ugh";
            }

            SendMessage(msg.ToNetMQMessage());
        }

        public void DeleteFile(NetMQMessage message)
        {
            Message<BucketMessage> msg = new Message<BucketMessage>(message);
            string relativePath = msg.Data.Path;
            string userGuid = MessageHelper.Decode(msg.Data.Token); ;
            string absolutePath = _path + userGuid + relativePath;

            msg.Data.Successful = FileSystemService.DeleteFile(absolutePath);
            msg.Topic = "Response";

            if (!msg.Data.Successful)
            {
                msg.Data.ErrorMessage = "Ugh";
            }
            
            SendMessage(msg.ToNetMQMessage());
        }
    }
}
