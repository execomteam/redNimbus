using RedNimbus.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.API.Services.Interfaces
{
    public interface IUserService
    {
        
            User Create(User user);
            User Authenticate(User user);
        
    }
}
