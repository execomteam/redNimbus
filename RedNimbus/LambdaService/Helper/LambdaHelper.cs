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
        public Guid CreateLambda(Message<LambdaMessage> message)
        {
            Guid lambdaId = Guid.NewGuid();

            try
            {
                ExtractSourceFile(message.Bytes.ToByteArray(), lambdaId);
                GenerateDockerfile(message.Data.Runtime, lambdaId);
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
                default:
                    return extension;
            }

            var projectName = Directory.EnumerateFiles(path, extension, SearchOption.TopDirectoryOnly)
                           .Select(p => Path.GetFileNameWithoutExtension(p))
                           .ToList().FirstOrDefault().ToString();

            return projectName;
        }

        private string GetTemplateName(RuntimeType runtimeType)
        {
            switch (runtimeType)
            {
                case RuntimeType.Csharp:
                    return "CSHARP";
                case RuntimeType.Python:
                    return "PYTHON";
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

        public string ExecuteLambda(string guid)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo()
            {
                FileName = "docker",
                Arguments = $"run --rm {guid}",
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
    }
}
