using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RedNimbus.BucketService.Helper;
using NetMQ;
using RedNimbus.Communication;
using RedNimbus.BucketService.Services;

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
            string relativePath = MessageHelper.GetPath(message);
            string userGuid = MessageHelper.GetStringGuid(message);
            string absolutePath = _path + userGuid + relativePath;

            List<string> contentList = FileSystemService.ListContent(absolutePath);

            MessageHelper.InsertContentList(message, contentList);
            SendMessage(message);
        }

        public void CreateBucket(NetMQMessage message)
        {
            string relativePath = MessageHelper.GetPath(message);
            string userGuid = MessageHelper.GetStringGuid(message);
            string absolutePath = _path + userGuid + relativePath;

            bool successfulCreate = FileSystemService.CreateBucket(absolutePath);

            if (successfulCreate)
            {
                MessageHelper.SuccessfulMessage(message);
                SendMessage(message);
            }
            else
            {
                MessageHelper.UnsuccessfulMessage(message);
                SendMessage(message);
            }
        }

        //Bucket must be empty for deleting
        public void DeleteBucket(NetMQMessage message)
        {
            string relativePath = MessageHelper.GetPath(message);
            string userGuid = MessageHelper.GetStringGuid(message);
            string absolutePath = _path + userGuid + relativePath;

            bool successfulDelete = FileSystemService.DeleteBucket(absolutePath);

            if (successfulDelete)
            {
                MessageHelper.SuccessfulMessage(message);
                SendMessage(message);
            }
            else
            {
                MessageHelper.UnsuccessfulMessage(message);
                SendMessage(message);
            }
        }

        public void PutFile(NetMQMessage message)
        {
            string relativePath = MessageHelper.GetPath(message);
            string userGuid = MessageHelper.GetStringGuid(message);
            string absolutePath = _path + userGuid + relativePath;
            byte[] fileAsByteArray = MessageHelper.GetFileAsByteArray(message);

            FileSystemService.ByteArrayToFile(absolutePath, fileAsByteArray);

            MessageHelper.SuccessfulMessage(message);
            SendMessage(message);
        }

        public void GetFile(NetMQMessage message)
        {
            string relativePath = MessageHelper.GetPath(message);
            string userGuid = MessageHelper.GetStringGuid(message);
            string absolutePath = _path + userGuid + relativePath;

            byte[] fileAsByteArray = FileSystemService.FileToByteArray(absolutePath);

            MessageHelper.PutFileAsByteArray(message, fileAsByteArray);

            SendMessage(message);
        }

        public void DeleteFile(NetMQMessage message)
        {
            string relativePath = MessageHelper.GetPath(message);
            string userGuid = MessageHelper.GetStringGuid(message);
            string absolutePath = _path + userGuid + relativePath;

            FileSystemService.DeleteFile(absolutePath);

            MessageHelper.SuccessfulMessage(message);

            SendMessage(message);
        }
    }
}
