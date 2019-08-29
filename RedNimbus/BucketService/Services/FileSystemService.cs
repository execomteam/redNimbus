using System;
using System.Collections.Generic;
using System.IO;

namespace RedNimbus.BucketService.Services
{
    public static class FileSystemService
    {


        public static List<string> ListContent(string path)
        {
            List<string> returnValue = new List<string>();
            foreach (string entry in Directory.GetDirectories(path))
            {
                string[] val = entry.Split('/');
                returnValue.Add(val[val.Length-1]);
            }
            returnValue.Add("*");
            foreach (string entry in Directory.GetFiles(path))
            {
                string[] val = entry.Split('/');
                returnValue.Add(val[val.Length - 1]);
            }
            return returnValue;
        }

        public static bool CreateBucket(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    return false;
                }

                // Try to create the directory.
                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            finally { }

        }

        public static bool DeleteBucket(string path)
        {
            try
            {
                // Try to delete the directory.
                Directory.Delete(path);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            finally { }

        }

        public static void ByteArrayToFile(string path, byte[] fileAsByteArray)
        {
            File.WriteAllBytes(path, fileAsByteArray);
        }

        public static byte[] FileToByteArray(string path) => File.ReadAllBytes(path);

        public static void DeleteFile(string path)
        {
            File.Delete(path);
        }
    }
}
