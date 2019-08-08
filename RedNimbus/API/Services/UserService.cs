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
using RedNimbus.API.Helper;
using System.Text.RegularExpressions;

namespace RedNimbus.API.Services
{
    public interface IUserService
    {
        User Create(User user);
        User Authenticate(User user);
    }

    public class UserService : IUserService
    {
        private static readonly Dictionary<string, User> registeredUsers = new Dictionary<string, User>();
        private static int idCounter = 0;

        public UserService() {}

        private bool Validate(User user)
        {
            bool isValid = true;

            if (String.IsNullOrWhiteSpace(user.Email)
                || String.IsNullOrWhiteSpace(user.Password)
                || String.IsNullOrWhiteSpace(user.FirstName)
                || String.IsNullOrWhiteSpace(user.LastName)) 
            {
                isValid = false;
            }

            if(!RegexUtilities.IsValidEmail(user.Email))
            {
                isValid = false;
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,24}");
            var hasLowerChar = new Regex(@"[a-z]+");

            if (!((hasNumber.IsMatch(user.Password)) && (hasUpperChar.IsMatch(user.Password)) && (hasMiniMaxChars.IsMatch(user.Password)) && (hasLowerChar.IsMatch(user.Password))))
            {
                isValid = false;
            }

            if(!(Regex.IsMatch(user.FirstName, @"^[a-zA-Z]+$") && Regex.IsMatch(user.LastName, @"^[a-zA-Z]+$")))
            {
                isValid = false;
            }


            return isValid;
        }

        public User Create(User user)
        {
            if (!Validate(user) || registeredUsers.ContainsKey(user.Email))
                return null;

            user.Id = idCounter++;
            user.Password = HashService.ComputeSha256Hash(user.Password);
            registeredUsers.Add(user.Email, user);

            return registeredUsers[user.Email];
        }

        public User Authenticate(User user)
        {
            if(!(String.IsNullOrWhiteSpace(user.Email) || String.IsNullOrWhiteSpace(user.Password)))
            {
                if (registeredUsers.ContainsKey(user.Email))
                {
                    var registeredUser = registeredUsers[user.Email];
                    if (registeredUser.Password == HashService.ComputeSha256Hash(user.Password))
                    {
                        return registeredUser;
                    }
                }
            }
            
            return null;
        }
    }
}
