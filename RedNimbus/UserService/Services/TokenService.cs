using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedNimbus.UserService.Helper;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using RedNimbus.UserService.Services.Interfaces;

namespace RedNimbus.UserService.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtConfiguration _jwtConfiguration;
        public TokenService(JwtConfiguration jwtConfiguration)
        {
            _jwtConfiguration = jwtConfiguration;
        }

        
        public string GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(_jwtConfiguration.Issuer,
              _jwtConfiguration.Issuer,
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
