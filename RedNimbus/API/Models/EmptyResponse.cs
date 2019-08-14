﻿using RedNimbus.API.Interfaces;
using System.Net;

namespace RedNimbus.API.Models
{
    public class EmptyResponse : IResponseData
    {
        public EmptyResponse()
        {

        }

        public HttpStatusCode StatusCode { get; set; }
        public object Value { get; set; }
    }
}
