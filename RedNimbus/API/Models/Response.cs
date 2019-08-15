using RedNimbus.API.Interfaces;
using System.Net;

namespace RedNimbus.API.Models
{
    public class Response<TValue> : IResponse<TValue>
    {
        public HttpStatusCode StatusCode { get; set; }
        public TValue Value { get; set; }
    }
}
