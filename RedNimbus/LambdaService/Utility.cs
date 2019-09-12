using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using NetMQ;
using RedNimbus.Communication;
using RedNimbus.Messages;

namespace RedNimbus.LambdaService
{
    public static class Utility
    {
        public static Guid CreateLambda(Message<LambdaMessage> message)
        {
            Guid guid = Guid.NewGuid();

            ExtractSourceFile(message.Bytes.ToByteArray(), guid);

            if (message.Data.Runtime == LambdaMessage.Types.RuntimeType.Csharp)
                GenerateAspNetDockerfile(guid);
            else if (message.Data.Runtime == LambdaMessage.Types.RuntimeType.Python)
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

            // TODO: Add error handling

            using (StreamWriter sw = File.CreateText(path + "Dockerfile"))
            {
                sw.WriteLine("FROM mcr.microsoft.com/dotnet/core/runtime:2.1-stretch-slim AS base");
                sw.WriteLine("WORKDIR /app");
                sw.WriteLine("FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build");
                sw.WriteLine("WORKDIR /src");
                sw.WriteLine($"COPY [\"{projectName}.csproj\", \"\"]");
                sw.WriteLine($"RUN dotnet restore \"./{projectName}.csproj\"");
                sw.WriteLine("COPY . .");
                sw.WriteLine("WORKDIR \"/src/.\"");
                sw.WriteLine($"RUN dotnet build \"{projectName}.csproj\" -c Release -o /app");
                sw.WriteLine("FROM build AS publish");
                sw.WriteLine($"RUN dotnet publish \"{projectName}.csproj\" -c Release -o /app");
                sw.WriteLine("FROM base AS final");
                sw.WriteLine("WORKDIR /app");
                sw.WriteLine("COPY --from=publish /app .");
                sw.WriteLine($"ENTRYPOINT [\"dotnet\", \"{projectName}.dll\"]");
            }
        }

        private static void GeneratePythonDockerfile(Guid guid)
        {
            string path = $".\\{guid}\\";

            // TODO: Add error handling

            var projectName = Directory.EnumerateFiles(path, "main.py", SearchOption.TopDirectoryOnly)
                                       .Select(p => Path.GetFileNameWithoutExtension(p))
                                       .ToList()
                                       .FirstOrDefault()
                                       .ToString();

            using (StreamWriter sw = File.CreateText(path + "Dockerfile"))
            {
                sw.WriteLine("FROM python:3");
                sw.WriteLine("WORKDIR /usr/src/app");
                sw.WriteLine("COPY requirements.txt ./");
                sw.WriteLine("RUN pip install --no-cache-dir -r requirements.txt");
                sw.WriteLine("COPY . .");
                sw.WriteLine("CMD [ \"python\", \"./main.py\" ]");
            }
        }

        private static void BuildImage(Guid guid)
        {
            Console.WriteLine("Building image...");
            Console.WriteLine(guid);
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

            string result = string.Empty;

            using (var process = new Process())
            {
                process.StartInfo = processInfo;

                process.Start();

                result = process.StandardOutput.ReadToEnd().Trim();

                Console.WriteLine(result);
                process.WaitForExit();
                Console.WriteLine("Exited");

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
