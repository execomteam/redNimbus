using RedNimbus.API.Interfaces;
using System.Threading.Tasks;

namespace RedNimbus.API.Services.Interfaces
{
    public interface ICommunicationService
    {
        Task<TResponseData> Send<TRequestData, TResponseData>(string path, TRequestData data) where TResponseData : IResponseData, new();
    }
}
