using System;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using RedNimbus.LambdaService.Services.Interfaces;
using RedNimbus.Messages;
using RedNimbus.Communication;
using static RedNimbus.Messages.LambdaMessage.Types;

namespace RedNimbus.LambdaService.Helper
{
    public class LambdaHelper : ILambdaHelper
    {
        public Guid CreateLambda(Message<LambdaMessage> lambdaMessage)
        {
            Guid lambdaId = Guid.NewGuid();
            var sourceFile = lambdaMessage.Bytes.ToByteArray();
            var runtime = lambdaMessage.Data.Runtime;

            try
            {
                ExtractSourceFile(sourceFile, lambdaId);
                GenerateDockerfile(runtime, lambdaId);
                BuildImage(lambdaId);
                RemoveSourceFile(lambdaId);
            }
            catch(Exception)
            {
                RemoveSourceFile(lambdaId);
                lambdaId = Guid.Empty;
            }

            return lambdaId;
        }

        #region Create lambda logic

        private void ExtractSourceFile(byte[] file, Guid guid)
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

        private void GenerateDockerfile(RuntimeType runtimeType, Guid guid)
        {
            string sourcePath = $".\\{guid}\\";
            string projectName = GetProjectName(sourcePath, runtimeType);
            string templateName = GetTemplateName(runtimeType);

            if (string.IsNullOrWhiteSpace(projectName) || string.IsNullOrWhiteSpace(templateName))
                throw new Exception();

            using (StreamWriter sw = File.CreateText(sourcePath + "Dockerfile"))
            {
                string content = File.ReadAllText($".\\Resources\\{templateName}");          
                sw.Write(string.Format(content, projectName));
            }
        }

        private string GetProjectName(string path, RuntimeType runtimeType)
        {
            string extension = string.Empty;

            switch(runtimeType)
            {
                case RuntimeType.Csharp:
                    extension = "*.csproj";
                    break;
                case RuntimeType.Python:
                    extension = "main.py";
                    break;
                case RuntimeType.Node:
                    extension = "app.js";
                    break;
                case RuntimeType.Go:
                    extension = "main.go";
                    break;
                default:
                    return extension;
            }

            return Directory.EnumerateFiles(path, extension, SearchOption.TopDirectoryOnly)
                           .Select(p => Path.GetFileNameWithoutExtension(p))
                           .ToList().FirstOrDefault().ToString();
        }

        private string GetTemplateName(RuntimeType runtimeType)
        {
            switch (runtimeType)
            {
                case RuntimeType.Csharp:
                    return "CSHARP";
                case RuntimeType.Python:
                    return "PYTHON";
                case RuntimeType.Node:
                    return "NODE";
                case RuntimeType.Go:
                    return "GO";
                default:
                    return "";
            }
        }

        private void BuildImage(Guid guid)
        {
            Console.WriteLine($"Building image {guid}...");

            string sourcePath = $".\\{guid}\\";

            ProcessStartInfo processInfo = new ProcessStartInfo()
            {
                WorkingDirectory = sourcePath,
                FileName = "docker",
                Arguments = $"build -t {guid} .",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            int exitCode;

            using (var process = new Process())
            {
                process.StartInfo = processInfo;

                process.Start();

                string result = process.StandardOutput.ReadToEnd().Trim();

                process.WaitForExit();
                exitCode = process.ExitCode;

                if (!process.HasExited)
                    process.Kill();

                process.Close();
            }

            Console.WriteLine($"Image building process finished with exit code {exitCode}.");

            if (exitCode != 0)
                throw new Exception();
        }

        private void RemoveSourceFile(Guid guid)
        {
            string sourcePath = $".\\{guid}";
            if(Directory.Exists(sourcePath))
                Directory.Delete(sourcePath, true);
        }

        #endregion

        public string ExecuteGetLambda(string lambdaId)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo()
            {
                FileName = "docker",
                Arguments = $"run --rm {lambdaId}",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            string result = string.Empty;

            using (var process = new Process())
            {
                process.StartInfo = processInfo;

                process.Start();

                result = process.StandardOutput.ReadToEnd().Trim();

                process.WaitForExit();

                if (!process.HasExited)
                    process.Kill();

                process.Close();
            }

            return result;
        }

        public byte[] ExecutePostLambda(Message<LambdaMessage> lambdaMessage)
        {
            string absolutePath = AppDomain.CurrentDomain.BaseDirectory + "data";

            // TODO: Add request id from message after merging with development
            // var requestId = System.Text.Encoding.UTF8.GetString(message.Id.ToByteArray(), 0, message.Id.ToByteArray().Length);

            var requestId = Guid.NewGuid();
            var lambdaId = lambdaMessage.Data.Guid;

            var targetPath = absolutePath + Path.DirectorySeparatorChar + requestId + Path.DirectorySeparatorChar;
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            File.WriteAllBytes(targetPath + "in", lambdaMessage.Bytes.ToByteArray());

            ProcessStartInfo processInfo = new ProcessStartInfo()
            {
                FileName = "docker",
                Arguments = $"run --rm -v {absolutePath}:/app/data {lambdaId} {requestId}",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            string result = string.Empty;

            using (var process = new Process())
            {
                process.StartInfo = processInfo;

                process.Start();

                result = process.StandardOutput.ReadToEnd().Trim();

                process.WaitForExit();

                if (!process.HasExited)
                    process.Kill();

                process.Close();
            }

            byte[] payload = File.ReadAllBytes(targetPath + "out");
            Directory.Delete(targetPath, true);

            return payload;
        }
    }
}