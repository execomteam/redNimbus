using System.Net;

namespace RedNimbus.API.Services
{
    public interface IResponseData
    {
        HttpStatusCode StatusCode { get; set; }
    }
}