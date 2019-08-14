using System.Net;

namespace RedNimbus.API.Interfaces
{
    public interface IResponseData
    {
        HttpStatusCode StatusCode { get; set; }
        object Value { get; set; }
    }
}