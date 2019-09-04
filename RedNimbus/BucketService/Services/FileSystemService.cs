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
                    returnValue.Add(MessageHelper.GetNameFromPath(entry));
                }
                returnValue.Add("*");
                foreach (string entry in Directory.GetFiles(path))
                {
                    returnValue.Add(MessageHelper.GetNameFromPath(entry));
                }
            }
            catch (DirectoryNotFoundException)
            {
                
                return null;
            }
            return returnValue;
        }

        public static string CreateFolder(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    int i = 1;
                    string path1 = path + "(" + i + ")";

                    while (Directory.Exists(path1))
                    {
                        i++;
                        path1 = path + "(" + i + ")";

                    }
                    path = path1;
                }

                // Try to create the directory.
                Directory.CreateDirectory(path);

                return MessageHelper.GetNameFromPath(path);
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static bool DeleteFolder(string path)
        {
            try
            {
                System.IO.Directory.Delete(path, true);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (PathTooLongException)
            {
                return false;
            }
            catch (DirectoryNotFoundException)
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
