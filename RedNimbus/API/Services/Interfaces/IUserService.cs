using RedNimbus.API.Model;

namespace RedNimbus.API.Services.Interfaces
{
    public interface IUserService
    {
        User Create(User user);
        User Authenticate(User user);
    }
}
