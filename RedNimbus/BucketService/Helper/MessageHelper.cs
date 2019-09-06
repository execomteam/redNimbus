using RedNimbus.Communication;
using RedNimbus.Messages;
using RedNimbus.TokenManager;
using System;

namespace RedNimbus.BucketService.Helper
{
    public class MessageHelper
    {
        
        public static string GetAbsolutePath(string rootPath, Message<BucketMessage> msg, ITokenManager tokenManager)
        {
            string relativePath = msg.Data.Path;
            Guid guid = tokenManager.ValidateToken(msg.Data.Token);
            if (guid.Equals(Guid.Empty))
            {
                return null;
            }

            return rootPath + guid.ToString("B") + relativePath;

        }

        public static string GetNameFromPath(string path)
        {
            string[] val = path.Split('/');
            string last = val[val.Length - 1];
            string[] splitLast = last.Split('\\');
            return splitLast[splitLast.Length - 1];
        }

    }
}