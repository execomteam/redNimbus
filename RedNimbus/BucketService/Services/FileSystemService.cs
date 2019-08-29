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
            try
            {
                foreach (string entry in Directory.GetDirectories(path))
                {
                    //TODO: Change this shit
                    string[] val = entry.Split('/');
                    string last = val[val.Length - 1];
                    string[] splitLast = last.Split('\\');
                    returnValue.Add(splitLast[splitLast.Length - 1]);
                }
                returnValue.Add("*");
                foreach (string entry in Directory.GetFiles(path))
                {
                    //TODO: Change this shit
                    string[] val = entry.Split('/');
                    string last = val[val.Length - 1];
                    string[] splitLast = last.Split('\\');
                    returnValue.Add(splitLast[splitLast.Length - 1]);
                }
            }
            catch (Exception)
            {
                return null;
            }
            finally { }
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
            catch (Exception)
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
            catch (Exception)
            {
                return false;
            }
            finally { }

        }

        public static bool ByteArrayToFile(string path, byte[] fileAsByteArray)
        {
            try
            {
                File.WriteAllBytes(path, fileAsByteArray);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally { }
        }

        public static byte[] FileToByteArray(string path)
        {
            try
            {
                return File.ReadAllBytes(path);
            }
            catch (Exception)
            {
                return null;
            }
            finally { }
        }

        public static bool DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally { }
        }
    }
}
