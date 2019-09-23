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

        private void Log(Guid id, string origin, string payload = null, LogMessage.Types.LogType type = LogMessage.Types.LogType.Info)
        {
            _logSender.Send(id, new LogMessage()
            {
                Date = DateTime.Now.ToShortDateString().ToString(),
                Time = DateTime.Now.TimeOfDay.ToString(),
                Type = LogMessage.Types.LogType.Info,
                Payload = payload == null ? "some file(s)" : payload,
                Origin = origin
            });
        }

        [HttpGet]
        public IActionResult Get()
        {
            Guid requestId = Guid.NewGuid();
            Log(requestId, "BucketControler/Get - Request received");

            return _bucketService.Get(Request.Headers["token"], requestId)
                .Map((x) => AllOk(x), (x) => Log(requestId, "BucketControler/Get - Get bucket successful", x.ToString()))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError, (x) => Log(requestId, "BucketControler/Get - Can't find bucket", Request.Headers["token"]))
                .Reduce(InternalServisErrorHandler, (x) => Log(requestId, "BucketControler/Get - InternalError", x.Message));
        }

        [HttpPost("post")]
        public IActionResult ListBucketContent([FromBody]StringDto val)
        {
            Guid requestId = Guid.NewGuid();
            Log(requestId, "BucketControler/ListBucketContent - Request received", val.Path + val.Value);

            return _bucketService.ListBucketContent(Request.Headers["token"], val.Path, requestId)
                .Map((x) => AllOk(x), (x) => Log(requestId, "BucketControler/ListBucketContent - Request received", x.Count.ToString() + " file(s)"))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError, (x) => Log(requestId, "BucketController/ListBucketContent - Can't find bucket's content", val.Path + val.Value))
                .Reduce(InternalServisErrorHandler, (x) => Log(requestId, "BucketControler/ListBucketContent - InternalError", val.Path + val.Value));
        }


        [HttpPost("createBucket")]
        public IActionResult CreateBucket([FromBody]StringDto bucketName)
        {
            Guid requestId = Guid.NewGuid();
            Log(requestId, "BucketControler/CreateBucket - Request received", bucketName.Path + bucketName.Value);

            return _bucketService.CreateBucket(Request.Headers["token"], bucketName, requestId)
                .Map((x) => AllOk(x), (x) => Log(requestId, "BucketControler/CreateBucket - Request received", x.Path + x.Value))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError, (x) => Log(requestId, "BucketControler/CreateBucket - Can't create bucket", bucketName.Path + bucketName.Value))
                .Reduce(InternalServisErrorHandler, (x) => Log(requestId, "BucketControler/CreateBucket - InternalError", bucketName.Path + bucketName.Value));
        }

        [HttpPost("deleteBucket")]
        public IActionResult DeleteBucket([FromBody]StringDto bucketName)
        {
            Guid requestId = Guid.NewGuid();
            Log(requestId, "BucketControler/DeleteBucket - Request received", bucketName.Path + bucketName.Value);

            return  _bucketService.DeleteBucket(Request.Headers["token"], bucketName, requestId)
                .Map((x) => AllOk(x), (x) => Log(requestId, "BucketControler/DeleteBucket - Request received", bucketName.Path + bucketName.Value))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError, (x) => Log(requestId, "BucketControler/DeleteBucket - Can't delete bucket", bucketName.Path + bucketName.Value))
                .Reduce(InternalServisErrorHandler, (x) => Log(requestId, "BucketControler/Delete - InternalError", bucketName.Path + bucketName.Value));
        }


        [HttpPost("uploadFile")]
        [RequestSizeLimit(350001000)]
        public IActionResult UploadFile([FromForm]UploadFileDto uploadFile)
        {
            Guid requestId = Guid.NewGuid();
            Log(requestId, "BucketControler/UploadFile - Request received", string.Format("{0} bytes received", uploadFile.File.Length));

            return _bucketService.UploadFile(Request.Headers["token"], uploadFile, requestId)
                .Map((x) => AllOk(x), (x) => Log(requestId, "BucketControler/UploadFile - Request received", string.Format("{0} bytes to upload", x.File.Length)))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError, (x) => Log(requestId, "BucketControler/UploadFile - Can't upload bucket", string.Format("{0} bytes to upload", uploadFile.File.Length)))
                .Reduce(InternalServisErrorHandler, (x) => Log(requestId, "BucketControler/UploadFile - InternalError", string.Format("{0} bytes to upload", uploadFile.File.Length)));
        }

        [HttpPost("deleteFile")]
        public IActionResult DeleteFile([FromBody]StringDto fileName)
        {
            Guid requestId = Guid.NewGuid();

            return _bucketService.DeleteFile(Request.Headers["token"], fileName, requestId)
                .Map((x) => AllOk(x), (x) => Log(requestId, "BucketControler/DeleteBucket - Request received", x.Path))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError, (x) => Log(requestId, "BucketControler/DeleteFile - Can't upload bucket", fileName.Path))
                .Reduce(InternalServisErrorHandler, (x) => Log(requestId, "BucketControler/DeleteFile - InternalError", string.Format("{0} bytes to upload", fileName.Path)));
        }

        [HttpPost("downloadFile")]
        public IActionResult DownloadFile([FromBody]StringDto fileName)
        {
            Guid requestId = Guid.NewGuid();
            Log(requestId, "BucketControler/DownloadFile - Request received", fileName.Path + fileName.Value);

            return _bucketService.DownloadFile(Request.Headers["token"], fileName, requestId)
                .Map((x) => (IActionResult)File(x.File, x.Type, x.Value), (x) => Log(requestId, "BucketControler/DownloadBucket - Request received", string.Format("{0} bytes to download", x.File.Length)))
                .Reduce(NotFoundErrorHandler, x => x is NotFoundError, (x) => Log(requestId, "BucketControler/DownloadFile - Can't upload bucket", fileName.Path + fileName.Value))
                .Reduce(InternalServisErrorHandler, (x) => Log(requestId, "BucketControler/DownloadFile - InternalError", fileName.Path + fileName.Value));
        }
    }
}