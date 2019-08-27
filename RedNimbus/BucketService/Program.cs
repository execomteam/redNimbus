using System;
using RedNimbus.BucketService.Helper;
using RedNimbus.BucketService.Services;

namespace RedNimbus.BucketService
{
    class Program
    {
        static void Main(string[] args)
        {
            Services.BucketService bucketService = new Services.BucketService("/");
            bucketService.Start();
            bucketService.Subscribe("bucket/listBucketContent", bucketService.ListBucketContent);
            bucketService.Subscribe("bucket/createBucket", bucketService.CreateBucket);
            bucketService.Subscribe("bucket/deleteBucket", bucketService.CreateBucket);
            bucketService.Subscribe("bucket/putFile", bucketService.PutFile);
            bucketService.Subscribe("bucket/getFile", bucketService.GetFile);
            bucketService.Subscribe("bucket/deleteFile", bucketService.DeleteFile);
            while(true)
            {

            }

        }
    }
}
