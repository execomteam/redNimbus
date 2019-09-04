using RedNimbus.BucketService.Helper;
using System;
using System.Collections.Generic;
using System.IO;

namespace RedNimbus.BucketService.Services
{
    public static class FileSystemService
    {
        /*
        public static bool CreateUser(string path)
        {
            try
            {
                string pathForZfs = path.TrimStart('/');
                string cmd = "zfs create " + pathForZfs;
                ShellHelper.Bash(cmd);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public static bool CreateBucket(string path)
        {
            try
            {
                string pathForZfs = path.TrimStart('/');
                string cmd = "zfs create " + pathForZfs;
                ShellHelper.Bash(cmd);
                cmd = "zfs set quota=5G " + pathForZfs;
                ShellHelper.Bash(cmd);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        */

        public static int NumberOfDirectories(string path)
        {
            return Directory.GetDirectories(path).Length;
        }

        public static List<string> ListContent(string path)
        {
            List<string> returnValue = new List<string>();
            try
            {
                foreach (string entry in Directory.GetDirectories(path))
                {
                    //TODO: Find better solution for this, later.
                    string[] val = entry.Split('/');
                    string last = val[val.Length - 1];
                    string[] splitLast = last.Split('\\');
                    returnValue.Add(splitLast[splitLast.Length - 1]);
                }
                returnValue.Add("*");
                foreach (string entry in Directory.GetFiles(path))
                {
                    //TODO: Find better solution for this, later.
                    string[] val = entry.Split('/');
                    string last = val[val.Length - 1];
                    string[] splitLast = last.Split('\\');
                    returnValue.Add(splitLast[splitLast.Length - 1]);
                }
            }
            catch (DirectoryNotFoundException)
            {
                
                return null;
            }
            return returnValue;
        }

        public static bool CreateFolder(string path)
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

        }

        public static bool DeleteFolder(string path)
        {
            try
            {
                bool successful = true;
                foreach (string entry in Directory.GetFiles(path))
                {
                    successful &= DeleteFile(entry);
                }
                foreach (string entry in Directory.GetDirectories(path))
                {
                    successful &= DeleteFolder(entry);
                }
                // Try to delete the directory.
                if (CheckFolderEmpty(path))
                {
                    Directory.Delete(path);
                    return successful;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static bool CheckFolderEmpty(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            var folder = new DirectoryInfo(path);
            if (folder.Exists)
            {
                return folder.GetFileSystemInfos().Length == 0;
            }

            throw new DirectoryNotFoundException();
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
        }
    }
}
