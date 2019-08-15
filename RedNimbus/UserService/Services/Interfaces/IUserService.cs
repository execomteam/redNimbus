using RedNimbus.UserService.Model;

namespace RedNimbus.UserService.Services.Interfaces
{
    public interface IUserService
    {
        User Create(User user);
        User Authenticate(User user);
        void AddAuthenticatedUser(string token, string email);
        User GetUserByToken(string token);
    }
}
