using RedNimbus.API.DTO;
using RedNimbus.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.API.Services
{
    public interface IUserService
    {
        User Create(User user);
        UserLoginDTO Login(UserLoginDTO udto);
    }

    public class UserService : IUserService
    {
        private Dictionary<string, User> registratedUsers;

        public UserService()
        { 
            registratedUsers = new Dictionary<string, User>();
        }

        private bool CheckRegistrationData(User user)
        {
            if (String.IsNullOrWhiteSpace(user.Email)
                || String.IsNullOrWhiteSpace(user.Password)
                || String.IsNullOrWhiteSpace(user.FirstName)
                || String.IsNullOrWhiteSpace(user.SecondName))
            {
                return false;
            }
            return true;
        }

        public User Create(User user)
        {
            if (!CheckRegistrationData(user))
            {
                return null;
            }

            if (registratedUsers.ContainsKey(user.Email))
            {
                return null;
            }

            user.Password = HashServices.ComputeSha256Hash(user.Password);
            registratedUsers.Add(user.Email, user);
            return user;
        }

        public UserLoginDTO Login(UserLoginDTO udto)
        {
            if (registratedUsers.ContainsKey(udto.Email))
            {
                var tempUser = registratedUsers[udto.Email];
                if (tempUser.Password == HashServices.ComputeSha256Hash(udto.Password))
                {
                    return udto;
                }
            }
            return null;
        }
    }
}
