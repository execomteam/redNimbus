using System;
using RedNimbus.BucketService.Helper;
using RedNimbus.BucketService.Services;

namespace RedNimbus.BucketService
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Services.BucketService bucketService = new Services.BucketService(ConfigHelper.LoadConfig(), new TokenManager.TokenManager(), 350*1024*1024);
                bucketService.Start();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("press any button");
                Console.ReadLine();
                return;
            }          
        }
    }
}
