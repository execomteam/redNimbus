using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using NetMQ;
using RedNimbus.Communication;
using RedNimbus.Messages;

using static RedNimbus.Messages.LambdaMessage.Types;

namespace RedNimbus.LambdaService
{
    public static class Utility
    {
        public static Guid CreateLambda(Message<LambdaMessage> message)
        {
            Guid guid = Guid.NewGuid();

            ExtractSourceFile(message.Bytes.ToByteArray(), guid);

            if (message.Data.Runtime == RuntimeType.Csharp)
                GenerateAspNetDockerfile(guid);
            else if (message.Data.Runtime == RuntimeType.Python)
                GeneratePythonDockerfile(guid);

            BuildImage(guid);

            RemoveSourceFile(guid);

            return guid;
        }

        private static void ExtractSourceFile(byte[] file, Guid guid)
        {
            string targetPath = $".\\{guid}";

            using (var stream = new MemoryStream(file))
            {
                using (var archive = new ZipArchive(stream))
                {
                    archive.ExtractToDirectory(targetPath);
                }
            }
        }

        private static void GenerateAspNetDockerfile(Guid guid)
        {
            string path = $".\\{guid}\\";

            var projectName = Directory.EnumerateFiles(path, "*.csproj", SearchOption.TopDirectoryOnly)
                                       .Select(p => Path.GetFileNameWithoutExtension(p))
                                       .ToList()
                                       .FirstOrDefault()
                                       .ToString();

            using (StreamWriter sw = File.CreateText(path + "Dockerfile"))
            {
                string content = File.ReadAllText(@".\Resources\CSHARP");

                sw.Write(string.Format(content, guid));
            }

            // TODO: Add error handling
        }

        private static void GeneratePythonDockerfile(Guid guid)
        {
            string path = $".\\{guid}\\";

            var projectName = Directory.EnumerateFiles(path, "main.py", SearchOption.TopDirectoryOnly)
                                       .Select(p => Path.GetFileNameWithoutExtension(p))
                                       .ToList()
                                       .FirstOrDefault()
                                       .ToString();

            using (StreamWriter sw = File.CreateText(path + "Dockerfile"))
            {
                string content = File.ReadAllText(@".\Resources\PYTHON");

                sw.Write(string.Format(content, guid));
            }

            // TODO: Add error handling
        }

        private static void BuildImage(Guid guid)
        {
            Console.WriteLine($"Building image {guid}...");

            string path = $".\\{guid}\\";

            ProcessStartInfo processInfo = new ProcessStartInfo()
            {
                WorkingDirectory = path,
                FileName = "docker",
                Arguments = $"build -t {guid} .",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = new Process())
            {
                process.StartInfo = processInfo;

                process.Start();

                string result = process.StandardOutput.ReadToEnd().Trim();

                Console.WriteLine(result);
                process.WaitForExit();
                Console.WriteLine("Process Exited");

                if (!process.HasExited)
                    process.Kill();

                process.Close();
            }

            Console.WriteLine("Building image finished");
        }

        private static void RemoveSourceFile(Guid guid)
        {
            string targetPath = $".\\{guid}";

            Directory.Delete(targetPath, true);
        }
    }
}
