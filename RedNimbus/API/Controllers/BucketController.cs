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
            List<string> toReturn;
            BucketMessage Data;
            using (var requestSocket = new RequestSocket("tcp://localhost:8000"))
            {

                NetMQMessage netMqMsg = new NetMQMessage();
                netMqMsg.Append("bucket/listBucketContent");

                BucketMessage msg = new BucketMessage();
                msg.Path = "/";
                //msg.Token = Request.Headers["token"];

                MemoryStream stream = new MemoryStream();
                msg.WriteTo(stream);

                NetMQFrame dataFrame = new NetMQFrame(stream.ToArray());
                netMqMsg.Append(dataFrame);

                requestSocket.SendMultipartMessage(netMqMsg);

                NetMQMessage responseMessage = requestSocket.ReceiveMultipartMessage();
                NetMQFrame data = responseMessage.Pop();
                Data = new BucketMessage();
                Data.MergeFrom(data.ToByteArray());

            }
            if (Data.Successful)
            {
                toReturn = new List<string>(Data.ReturnItems);
                return AllOk(toReturn);
            }
            return BadRequest();
        }
        /*
        [HttpGet("{id}")]
        public IActionResult ListBucketContent(string id)
        {
            List<string> toReturn;
            BucketMessage Data;
            using (var requestSocket = new RequestSocket("tcp://localhost:8000"))
            {

                NetMQMessage netMqMsg = new NetMQMessage();
                netMqMsg.Append("bucket/listBucketContent");

                BucketMessage msg = new BucketMessage();
                msg.Path = "/" + id;
                //msg.Token = Request.Headers["token"];

                MemoryStream stream = new MemoryStream();
                msg.WriteTo(stream);

                NetMQFrame dataFrame = new NetMQFrame(stream.ToArray());
                netMqMsg.Append(dataFrame);

                requestSocket.SendMultipartMessage(netMqMsg);

                NetMQMessage responseMessage = requestSocket.ReceiveMultipartMessage();
                NetMQFrame data = responseMessage.Pop();
                Data = new BucketMessage();
                Data.MergeFrom(data.ToByteArray());

            }
            if (Data.Successful)
            {
                toReturn = new List<string>(Data.ReturnItems);
                return AllOk(toReturn);
            }
            return BadRequest();
        }
        */

        [HttpGet("vezba")]
        public IActionResult Vezba() {
            BucketMessage Data;
            using (var requestSocket = new RequestSocket("tcp://localhost:8000"))
            {

                NetMQMessage netMqMsg = new NetMQMessage();
                netMqMsg.Append("bucket/putFile");

                BucketMessage msg = new BucketMessage();
                msg.Path = "/proba.txt";
                //msg.Token = Request.Headers["token"];
                msg.File = ByteString.CopyFrom(System.IO.File.ReadAllBytes("C:\\Users\\praksa\\Desktop\\deoKod.txt"));
                MemoryStream stream = new MemoryStream();
                msg.WriteTo(stream);

                NetMQFrame dataFrame = new NetMQFrame(stream.ToArray());
                netMqMsg.Append(dataFrame);

                requestSocket.SendMultipartMessage(netMqMsg);

                NetMQMessage responseMessage = requestSocket.ReceiveMultipartMessage();
                NetMQFrame data = responseMessage.Pop();
                Data = new BucketMessage();
                Data.MergeFrom(data.ToByteArray());
            }
            return AllOk();
        }

    }
}