using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.RestUserService.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken();
    }
}
