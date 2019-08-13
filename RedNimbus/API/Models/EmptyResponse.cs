using RedNimbus.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RedNimbus.API.Models
{
    public class EmptyResponse : IResponseData
    {
        public EmptyResponse()
        {

        }

        public HttpStatusCode StatusCode { get; set; }
    }
}
