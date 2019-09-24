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
        private ILogSender _logSender;
        public BucketController(BucketService bucketService, ILogSender logSender)
        {
            _bucketService = bucketService;
            _logSender = logSender;
        }

        private void LogRequest(Guid id, string origin, string payload = null)
        {
            _logSender.Send(id, new LogMessage()
            {
                Date = DateTime.Now.ToShortDateString().ToString(),
                Time = DateTime.Now.TimeOfDay.ToString(),
                Type = LogMessage.Types.LogType.Info,
                Payload = payload == null ? "some file(s)" : payload,
                Origin = "UserController/Get"
            });
        }

        [HttpGet]
        public IActionResult Get()
        {
            Guid requestId = Guid.NewGuid();

            return _bucketService.Get(Request.Headers["token"], requestId)
                .Map((x) => AllOk(x), (x) => LogRequest(requestId, "BucketService/Get - Request received", x.ToString()))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }

        [HttpPost("post")]
        public IActionResult ListBucketContent([FromBody]StringDto val)
        {
            Guid requestId = Guid.NewGuid();

            return _bucketService.ListBucketContent(Request.Headers["token"], val.Path, requestId)
                .Map((x) => AllOk(x), (x) => LogRequest(requestId, "BucketService/ListBucketContent - Request received", x.Count.ToString() + " file(s)"))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }


        [HttpPost("createBucket")]
        public IActionResult CreateBucket([FromBody]StringDto bucketName)
        {
            Guid requestId = Guid.NewGuid();

            return _bucketService.CreateBucket(Request.Headers["token"], bucketName, requestId)
                .Map((x) => AllOk(x), (x) => LogRequest(requestId, "BucketService/CreateBucket - Request received", x.Path))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }

        [HttpPost("deleteBucket")]
        public IActionResult DeleteBucket([FromBody]StringDto bucketName)
        {
            Guid requestId = Guid.NewGuid();

            return  _bucketService.DeleteBucket(Request.Headers["token"], bucketName, requestId)
                .Map((x) => AllOk(x), (x) => LogRequest(requestId, "BucketService/DeleteBucket - Request received", x.Path))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }


        [HttpPost("uploadFile")]
        [RequestSizeLimit(367002600)]
        public IActionResult UploadFile([FromForm]UploadFileDto uploadFile)
        {
            Guid requestId = Guid.NewGuid();

            return _bucketService.UploadFile(Request.Headers["token"], uploadFile, requestId)
                .Map((x) => AllOk(x), (x) => LogRequest(requestId, "BucketService/DeleteBucket - Request received", x.Path))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }

        [HttpPost("deleteFile")]
        public IActionResult DeleteFile([FromBody]StringDto fileName)
        {
            Guid requestId = Guid.NewGuid();

            return _bucketService.DeleteFile(Request.Headers["token"], fileName, requestId)
                .Map((x) => AllOk(x), (x) => LogRequest(requestId, "BucketService/DeleteBucket - Request received", x.Path))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }

        [HttpPost("downloadFile")]
        public IActionResult DownloadFile([FromBody]StringDto fileName)
        {
            Guid requestId = Guid.NewGuid();

            return _bucketService.DownloadFile(Request.Headers["token"], fileName, requestId)
                .Map((x) => (IActionResult)File(x.File, x.Type, x.Value), (x) => LogRequest(requestId, "BucketService/DeleteBucket - Request received", fileName.Path))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError)
                .Reduce(InternalServisErrorHandler);
        }
    }
}