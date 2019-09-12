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

        
        
        
        [HttpGet("{id}")]
        public IActionResult ListBucketContent(string id) => 
            _bucketService.ListBucketContent(Request.Headers["token"], id)
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

    }
}