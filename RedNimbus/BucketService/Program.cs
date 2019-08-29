using System;
using RedNimbus.BucketService.Helper;
using RedNimbus.BucketService.Services;

namespace RedNimbus.BucketService
{
    class Program
    {
        static void Main(string[] args)
        {
            Services.BucketService bucketService = new Services.BucketService("C:/Users/praksa/Desktop/");
            bucketService.Start();
        }
    }
}
