using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using RedNimbus.UserService.Model;
using RedNimbus.UserService.Services.Interfaces;
using RedNimbus.UserService.Helper;

namespace RedNimbus.UserService.Services
{
    public class UserService : IUserService
    {
        private static readonly Dictionary<string, User> registeredUsers = new Dictionary<string, User>();
        private static readonly Dictionary<string, string> tokenEmailPairs = new Dictionary<string, string>();
        public UserService() {}

        #region Validation functions

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
            return Regex.IsMatch(user.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,24}$");
        }

        private bool IsNameValid(User user)
        {
            return Regex.IsMatch(user.FirstName, @"^[a-z A-Z]+$") && Regex.IsMatch(user.LastName, @"^[a-z A-Z]+$");
        }

        private bool IsPhoneValid(User user)
        {   
            if(!String.IsNullOrWhiteSpace(user.PhoneNumber))
                return Regex.IsMatch(user.PhoneNumber, @"^[0-9()-]+$");

            return true;
        }

        private void CapitalizeFirstLetter(User user)
        {
            user.FirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.FirstName.ToLower());
            user.LastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.LastName.ToLower());
        }

        # endregion

        public User Create(User user)
        {
            if (!IsUserValid(user) || registeredUsers.ContainsKey(user.Email))
                return null;
            
            
            user.Id = Guid.NewGuid();
            user.Password = HashHelper.ComputeHash(user.Password);

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
                    if (registeredUser.Password == HashHelper.ComputeHash(user.Password))
                    {
                        return registeredUser;
                    }
                }
            }
            
            return null;
        }

        public void AddAuthenticatedUser(string token, string email)
        {
            if (tokenEmailPairs.ContainsKey(token))
                return;
            tokenEmailPairs.Add(token, email);
        }

        public User GetUserByToken(string token) {
            if (!tokenEmailPairs.ContainsKey(token))
                return null;

            string email = tokenEmailPairs[token];
            if (!registeredUsers.ContainsKey(email))
                return null;
            return registeredUsers[email];
        }
    }
}
