using RedNimbus.Communication;
using RedNimbus.Messages;

namespace RedNimbus.BucketService.Helper
{
    public static class MessageHelper
    {
        
        public static string GetAbsolutePath(string rootPath, Message<BucketMessage> msg)
        {
            string relativePath = msg.Data.Path;
            string userGuid = MessageHelper.Decode(msg.Data.Token);
            return rootPath + userGuid + relativePath;
        }
        
        //TODO: Decode token to get GUID
        public static string Decode(string token)
        {
            return "Spisak";
        }

    }
}