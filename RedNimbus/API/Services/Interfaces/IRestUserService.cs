using RedNimbus.API.Models;
using RedNimbus.Either;
using RedNimbus.Either.Errors;
using System.Threading.Tasks;

namespace RedNimbus.API.Services.Interfaces
{
    public interface IRestUserService
    {
        Task<Either<IError, TSuccess>> Get<TSuccess>(string path, string token);
        Task<Either<IError, TSuccess>> Send<TRequest, TSuccess>(string path, TRequest data);
    }
}
