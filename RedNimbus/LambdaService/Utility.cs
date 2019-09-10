using System;
using System.IO;
using System.IO.Compression;

namespace RedNimbus.LambdaService
{
    public static class Utility
    {
        public static void UnzipSource()
        {
            string sourcePath = @".\test.zip";
            string targetPath = @".\";

            targetPath = Path.GetFullPath(targetPath);



            throw new NotImplementedException();
        }

        public static void GenerateDockerfile()
        {
            throw new NotImplementedException();
        }

        public static void BuildImage()
        {
            throw new NotImplementedException();
        }
    }
}
