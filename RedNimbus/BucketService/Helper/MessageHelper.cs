using NetMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.BucketService.Helper
{
    public static class MessageHelper
    {
        public static string GetPath(NetMQMessage message)
        {
            return "";
        }

        public static string GetStringGuid(NetMQMessage message)
        {
            return "";
        }

        public static void InsertContentList(NetMQMessage message, List<string> contentList)
        {

        }

        public static string GetBucketName(NetMQMessage message)
        {
            return "";
        }

        public static void SuccessfulMessage(NetMQMessage message)
        {

        }

        public static void UnsuccessfulMessage(NetMQMessage message)
        {

        }

        public static byte[] GetFileAsByteArray(NetMQMessage message)
        {
            return null;
        }

        public static void PutFileAsByteArray(NetMQMessage message, byte[] fileAsByteArray)
        {

        }
    }
}