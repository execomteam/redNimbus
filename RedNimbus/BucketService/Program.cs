﻿using System;
using RedNimbus.BucketService.Helper;
using RedNimbus.BucketService.Services;

namespace RedNimbus.BucketService
{
    class Program
    {
        static void Main(string[] args)
        {
            Services.BucketService bucketService = new Services.BucketService(ConfigHelper.LoadConfig(), new TokenManager.TokenManager());
            //("C:/Users/praksa/Desktop/Spisak/",  new TokenManager.TokenManager());
            bucketService.Start();
        }
    }
}
