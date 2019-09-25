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
using RedNimbus.LogLibrary;

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
        public IActionResult Get()
        {
            Guid requestId = Guid.NewGuid();

            return _bucketService.Get(Request.Headers["token"], requestId)
                .Map((x) => AllOk(x))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }

        [HttpPost("post")]
        public IActionResult ListBucketContent([FromBody]StringDto val)
        {
            Guid requestId = Guid.NewGuid();
            
            return _bucketService.ListBucketContent(Request.Headers["token"], val.Path, requestId)
                .Map((x) => AllOk(x))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }


        [HttpPost("createBucket")]
        public IActionResult CreateBucket([FromBody]StringDto bucketName)
        {
            Guid requestId = Guid.NewGuid();
            
            return _bucketService.CreateBucket(Request.Headers["token"], bucketName, requestId)
                .Map((x) => AllOk(x))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }

        [HttpPost("deleteBucket")]
        public IActionResult DeleteBucket([FromBody]StringDto bucketName)
        {
            Guid requestId = Guid.NewGuid();
            
            return  _bucketService.DeleteBucket(Request.Headers["token"], bucketName, requestId)
                .Map((x) => AllOk(x))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }


        [HttpPost("uploadFile")]
        [RequestSizeLimit(367002600)]
        public IActionResult UploadFile([FromForm]UploadFileDto uploadFile)
        {
            Guid requestId = Guid.NewGuid();
            
            return _bucketService.UploadFile(Request.Headers["token"], uploadFile, requestId)
                .Map((x) => AllOk(x))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }

        [HttpPost("deleteFile")]
        public IActionResult DeleteFile([FromBody]StringDto fileName)
        {
            Guid requestId = Guid.NewGuid();

            return _bucketService.DeleteFile(Request.Headers["token"], fileName, requestId)
                .Map((x) => AllOk(x))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }

        [HttpPost("downloadFile")]
        public IActionResult DownloadFile([FromBody]StringDto fileName)
        {
            Guid requestId = Guid.NewGuid();
            
            return _bucketService.DownloadFile(Request.Headers["token"], fileName, requestId)
                .Map((x) => (IActionResult)File(x.File, x.Type, x.Value))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }
    }
}