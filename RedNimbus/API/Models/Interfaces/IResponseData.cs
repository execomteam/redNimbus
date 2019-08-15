using System.Net;

namespace RedNimbus.API.Interfaces
{
    public interface IResponse<TValue>
    {
        HttpStatusCode StatusCode { get; set; }
        TValue Value { get; set; }
    }
}