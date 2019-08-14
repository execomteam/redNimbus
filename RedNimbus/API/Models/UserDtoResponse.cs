using System.Net;
using RedNimbus.API.Interfaces;
using RedNimbus.DTO;

namespace RedNimbus.API.Models
{
    public class DtoResponse : IResponseData
    {
        public DtoResponse()
        {

        }


        public HttpStatusCode StatusCode { get; set; }
        public object Value { get; set; }
    }
}