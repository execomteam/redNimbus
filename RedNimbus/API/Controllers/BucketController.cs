using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetMQ;
using NetMQ.Sockets;
using RedNimbus.API.Controllers;
using RedNimbus.API.Services;
using RedNimbus.DTO;
using RedNimbus.Either.Errors;
using RedNimbus.Either;
using RedNimbus.Messages;
using DTO;

namespace RedNimbus.API.Controllers
{
    [ApiController]
    [Route("api/bucket")]
    public class BucketController : BaseController
    {
        private readonly BucketService _bucketService;
        public BucketController(BucketService bucketService)
        {
            _bucketService = bucketService;
        }

        [HttpGet]
        public IActionResult Get() =>
            _bucketService.Get(Request.Headers["token"])
                .Map((x) => AllOk(x))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);

        [HttpPost("post")]
        public IActionResult ListBucketContent([FromBody]StringDto val) => 
            _bucketService.ListBucketContent(Request.Headers["token"], val.Path)
                .Map((x) => AllOk(x))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        

        [HttpPost("createBucket")]
        public IActionResult CreateBucket([FromBody]StringDto bucketName) =>
            _bucketService.CreateBucket(Request.Headers["token"], bucketName)
                .Map((x) => AllOk(x))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);

        [HttpPost("deleteBucket")]
        public IActionResult DeleteBucket([FromBody]StringDto bucketName) =>
            _bucketService.DeleteBucket(Request.Headers["token"], bucketName)
                .Map((x) => AllOk(x))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        
        [HttpPost("uploadFile")]
        [RequestSizeLimit(350001000)]
        public IActionResult UploadFile([FromForm]UploadFileDto uploadFile) =>
            _bucketService.UploadFile(Request.Headers["token"], uploadFile)
                .Map((x) => AllOk(x))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);

        [HttpPost("deleteFile")]
        public IActionResult DeleteFile([FromBody]StringDto fileName) =>
            _bucketService.DeleteFile(Request.Headers["token"], fileName)
                .Map((x) => AllOk(x))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);

        [HttpPost("downloadFile")]
        public IActionResult DownloadFile([FromBody]StringDto fileName) =>
            _bucketService.DownloadFile(Request.Headers["token"], fileName)
                .Map((x) => (IActionResult)File(x.File, x.Type, x.Value))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);
    }
}