using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace RedNimbus.TokenManager
{
    public interface ITokenManager
    {
        string GenerateToken(Guid id);
        ClaimsPrincipal GetPrincipal(string token);
        Guid ValidateToken(string token);
    }
}
