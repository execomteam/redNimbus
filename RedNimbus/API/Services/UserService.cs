using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RedNimbus.API.DTO;
using RedNimbus.API.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedNimbus.API.Services
{
    public interface IUserService
    {
        User Create(User user);
        UserDTO Login(UserLoginDTO userLoginDTO);
    }

    public class UserService : IUserService
    {
        private static readonly Dictionary<string, User> registeredUsers = new Dictionary<string, User>();
        private readonly JwtConfiguration jwtConfiguration;

        public UserService(IOptions<JwtConfiguration> config)
        {
            jwtConfiguration = config.Value;
        }
      
        #region registration

        private bool CheckRegistrationData(User user)
        {
            if (String.IsNullOrWhiteSpace(user.Email)
                || String.IsNullOrWhiteSpace(user.Password)
                || String.IsNullOrWhiteSpace(user.FirstName)
                || String.IsNullOrWhiteSpace(user.LastName))
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

            if (registeredUsers.ContainsKey(user.Email))
            {
                return null;
            }

            user.Password = HashService.ComputeSha256Hash(user.Password);
            registeredUsers.Add(user.Email, user);
            return user;
        }

        #endregion

        #region login
        private string GenerateJwt()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(jwtConfiguration.Issuer,
              jwtConfiguration.Issuer,
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public UserDTO Login(UserLoginDTO userLoginDTO)
        {
            if (registeredUsers.ContainsKey(userLoginDTO.Email))
            {
                var user = registeredUsers[userLoginDTO.Email];
                if (user.Password == HashService.ComputeSha256Hash(userLoginDTO.Password))
                {
                    return new UserDTO(user.FirstName, user.LastName, user.Email, GenerateJwt());
                }
            }
            return null;
        }

        #endregion
    }
}
