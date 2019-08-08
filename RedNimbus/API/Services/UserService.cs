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
using System.Globalization;
using RedNimbus.API.Services.Interfaces;

namespace RedNimbus.API.Services
{
    

    public class UserService : IUserService
    {
        private static readonly Dictionary<string, User> registeredUsers = new Dictionary<string, User>();
        private static int idCounter = 0;

        public UserService() {}

        private bool IsUserValid(User user)
        {
            if(!IsEmpty(user)
                && IsEmailValid(user)
                && IsPasswordValid(user)
                && IsNameValid(user)
                && IsPhoneValid(user))
            {
                return true;
            }

            return false;
        }

        private bool IsEmpty(User user)
        {
            if (String.IsNullOrWhiteSpace(user.Email)
                || String.IsNullOrWhiteSpace(user.Password)
                || String.IsNullOrWhiteSpace(user.FirstName)
                || String.IsNullOrWhiteSpace(user.LastName))
            {
                return true;
            }
            return false;
        }

        private bool IsEmailValid(User user)
        {
            return RegexUtilities.IsValidEmail(user.Email);
        }

        private bool IsPasswordValid(User user)
        {
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,24}$");
            return regex.IsMatch(user.Password);
        }

        private bool IsNameValid(User user)
        {
            return (Regex.IsMatch(user.FirstName, @"^[a-zA-Z]+$") && Regex.IsMatch(user.LastName, @"^[a-zA-Z]+$"));
        }

        private bool IsPhoneValid(User user)
        {
            return Regex.IsMatch(user.PhoneNumber, @"^[0-9()-]+$");
        }

        private void CapitalizeFirstLetter(User user)
        {
            user.FirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.FirstName.ToLower());
            user.LastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.LastName.ToLower());
        }

        public User Create(User user)
        {
            if (!IsUserValid(user) || registeredUsers.ContainsKey(user.Email))
                return null;

            user.Id = idCounter++;
            user.Password = HashService.ComputeSha256Hash(user.Password);

            CapitalizeFirstLetter(user);
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
