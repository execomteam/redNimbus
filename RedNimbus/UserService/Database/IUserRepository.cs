using RedNimbus.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Database.Model;

namespace UserService.Database
{
    public interface IUserRepository
    {
        void SaveUser(User u);
        bool CheckIfExists(String email);
        User GetUserByEmail(String email);
    }
}
