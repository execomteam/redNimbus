using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace RedNimbus.LambdaService
{
    public static class Utility
    {
        public static void DoStuff()
        {
            UnzipSource();
            GeneratePythonDockerfile();
            BuildImage();
        }

        public static void UnzipSource()
        {
            Console.WriteLine("Extracting source...");

            string sourcePath = @".\Test.zip";
            string targetPath = @".\Test";

            targetPath = Path.GetFullPath(targetPath);
            if (!targetPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                targetPath += Path.DirectorySeparatorChar;

            //ZipFile.CreateFromDirectory(startPath, zipPath);
            ZipFile.ExtractToDirectory(sourcePath, targetPath);

            // TODO: Delete .zip file in case of successful extraction

            Console.WriteLine("Source extracted");
        }

        /// <summary>
        /// ASP.NET Core 2.1
        /// </summary>
        private static void GenerateAspNetDockerfile()
        {
            Console.WriteLine("Generating Dockerfile...");

            string path = @".\Test\";

            var projectName = Directory.EnumerateFiles(path, "*.csproj", SearchOption.TopDirectoryOnly)
                                       .Select(p => Path.GetFileNameWithoutExtension(p))
                                       .ToList()
                                       .FirstOrDefault()
                                       .ToString();

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

            Console.WriteLine("Dockerfile generated");
        }

        private static void GeneratePythonDockerfile()
        {
            string path = @".\Test\";

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

        public static void BuildImage()
        {
            Console.WriteLine("Building image...");

            string path = @".\Test\";

            ProcessStartInfo processInfo = new ProcessStartInfo()
            {
                WorkingDirectory = path,
                FileName = "docker",
                Arguments = $"build -t pytestimage .",
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

            Console.WriteLine("Building image finished");
        }
    }
}
