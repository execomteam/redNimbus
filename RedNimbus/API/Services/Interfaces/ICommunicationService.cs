using RedNimbus.API.Models;
using System.Threading.Tasks;

namespace RedNimbus.API.Services.Interfaces
{
    public interface ICommunicationService
    {
        Task<Response<TResponseData>> Send<TRequest, TResponseData>(string path, TRequest data);
    }
}
