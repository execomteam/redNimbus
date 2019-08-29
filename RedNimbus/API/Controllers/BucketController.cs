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
using RedNimbus.DTO;
using RedNimbus.Either.Errors;
using RedNimbus.Messages;

namespace API.Controllers
{
    [ApiController]
    [Route("api/bucket")]
    public class BucketController : ControllerBase
    {
        #region IActionResult

        private IActionResult AllOk()
        {
            return Ok(new Empty());
        }

        private IActionResult AllOk(object obj)
        {
            return Ok(obj);
        }

        private IActionResult BadRequestErrorHandler(IError error)
        {
            return BadRequest(error.Message);
        }

        private IActionResult InternalServisErrorHandler(IError error)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, error.Message);
        }

        private IActionResult NotFoundErrorHandler(IError error)
        {
            return NotFound(error.Message);
        }

        private IActionResult AuthenticationErrorHandler(IError error)
        {
            return StatusCode(StatusCodes.Status406NotAcceptable, error.Message);
        }

        #endregion

        [HttpGet]
        public IActionResult Get()
        {
            //Allocating variables
            NetMQMessage netMqMsg = new NetMQMessage();

            netMqMsg.Append("bucket/listBucketContent");

            //Initializating message 
            BucketMessage msg = new BucketMessage();
            msg.Path = "/";
            //msg.Token = Request.Headers["token"];

            //From BucketMessage to NetMqMessage
            MemoryStream stream = new MemoryStream();
            msg.WriteTo(stream);
            NetMQFrame dataFrame = new NetMQFrame(stream.ToArray());
            netMqMsg.Append(dataFrame);

            using (var requestSocket = new RequestSocket("tcp://localhost:8000"))
            {
                requestSocket.SendMultipartMessage(netMqMsg);
                netMqMsg = requestSocket.ReceiveMultipartMessage();
            }

            //From NetMqMessage to BucketMessage
            NetMQFrame recivedData = netMqMsg.Pop();
            BucketMessage Data = new BucketMessage();
            Data.MergeFrom(recivedData.ToByteArray());

            if (Data.Successful)
            {
                List<string>  toReturn = new List<string>(Data.ReturnItems);
                return AllOk(toReturn);
            }
            return BadRequest();
        }
        
        [HttpGet("{id}")]
        public IActionResult ListBucketContent(string id)
        {
            //Allocating variables
            NetMQMessage netMqMsg = new NetMQMessage();

            netMqMsg.Append("bucket/listBucketContent");

            //Initializating message 
            BucketMessage msg = new BucketMessage();
            msg.Path = "/" + id;
            //msg.Token = Request.Headers["token"];

            //From BucketMessage to NetMqMessage
            MemoryStream stream = new MemoryStream();
            msg.WriteTo(stream);
            NetMQFrame dataFrame = new NetMQFrame(stream.ToArray());
            netMqMsg.Append(dataFrame);

            using (var requestSocket = new RequestSocket("tcp://localhost:8000"))
            {
                requestSocket.SendMultipartMessage(netMqMsg);
                netMqMsg = requestSocket.ReceiveMultipartMessage();
            }

            //From NetMqMessage to BucketMessage
            NetMQFrame recivedData = netMqMsg.Pop();
            BucketMessage Data = new BucketMessage();
            Data.MergeFrom(recivedData.ToByteArray());

            if (Data.Successful)
            {
                List<string>  toReturn = new List<string>(Data.ReturnItems);
                return AllOk(toReturn);
            }
            return BadRequest();
        }
        
        [HttpPost("createBucket")]
        public IActionResult CreateBucket([FromBody]StringDto bucketName)
        {
            //Allocating variables
            NetMQMessage netMqMsg = new NetMQMessage();

            netMqMsg.Append("bucket/createBucket");

            //Initializating message 
            BucketMessage msg = new BucketMessage();
            msg.Path = "/" + bucketName.Value;
            //msg.Token = Request.Headers["token"];

            //From BucketMessage to NetMqMessage
            MemoryStream stream = new MemoryStream();
            msg.WriteTo(stream);
            NetMQFrame dataFrame = new NetMQFrame(stream.ToArray());
            netMqMsg.Append(dataFrame);

            using (var requestSocket = new RequestSocket("tcp://localhost:8000"))
            {
                requestSocket.SendMultipartMessage(netMqMsg);
                netMqMsg = requestSocket.ReceiveMultipartMessage();
            }

            //From NetMqMessage to BucketMessage
            NetMQFrame recivedData = netMqMsg.Pop();
            BucketMessage Data = new BucketMessage();
            Data.MergeFrom(recivedData.ToByteArray());

            if (Data.Successful)
            {
                return AllOk(bucketName);
            }
            return BadRequest();
        }
    }
}