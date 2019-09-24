using System;
using RedNimbus.BucketService.Helper;
using RedNimbus.BucketService.Services;
using RedNimbus.LogLibrary;

namespace RedNimbus.BucketService
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ILogSender sender = new LogSender("tcp://127.0.0.1:8887");
                Services.BucketService bucketService = new Services.BucketService(ConfigHelper.LoadConfig(), new TokenManager.TokenManager(), 350 * 1024 * 1024, sender);
                bucketService.Start();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("press any button to exit");
                Console.ReadLine();
                return;
            }          
        }
    }
}
