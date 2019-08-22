using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using RedNimbus.UserService.Model;
using RedNimbus.UserService.Services.Interfaces;
using RedNimbus.UserService.Helper;
using RedNimbus.Either.Errors;
using RedNimbus.Either;
using RedNimbus.DTO;

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
            return !IsEmpty(user) && IsEmailValid(user)
                && IsPasswordValid(user) && IsNameValid(user) && IsPhoneValid(user);

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

        # endregion


        public Either<IError, User> Create(User user)
        {
            if (IsEmpty(user))
            {
                return new Left<IError, User>(new UnacceptableFormatErr() { Message = "Requirerd field is empty!" });
            }

            if (!IsEmailValid(user))
            {
                return new Left<IError, User>(new UnacceptableFormatErr() { Message = "Email format is unacceptable!" });
            }

            if (!IsPasswordValid(user))
            {
                return new Left<IError, User>(new UnacceptableFormatErr() { Message = "Password format is unacceptable!" });
            }

            user.Id = Guid.NewGuid();
            user.Password = HashHelper.ComputeHash(user.Password);

            registeredUsers.Add(user.Email, user);

            return new Right<IError, User>(registeredUsers[user.Email]);
        }

        public Either<IError, User> Authenticate(User user)
        {

            if (!String.IsNullOrWhiteSpace(user.Email))
            {
                return new UnacceptableFormatErr() { Message = "Email field empty!" };
            }

            if (String.IsNullOrWhiteSpace(user.Password)){
                return new UnacceptableFormatErr() { Message = "Password field empty!" };
            }

            if (registeredUsers.ContainsKey(user.Email))
            {
                var registeredUser = registeredUsers[user.Email];
                if (registeredUser.Password == HashHelper.ComputeHash(user.Password))
                {
                    return registeredUser;
                }

            }

            return new UnacceptableFormatErr() { Message = "Email or Password is incorect" };

        }

        //public User Authenticate(User user)
        //{

        //    if(!(String.IsNullOrWhiteSpace(user.Email) || String.IsNullOrWhiteSpace(user.Password)))
        //    {
        //        if (registeredUsers.ContainsKey(user.Email))
        //        {
        //            var registeredUser = registeredUsers[user.Email];
        //            if (registeredUser.Password == HashHelper.ComputeHash(user.Password))
        //            {
        //                return registeredUser;
        //            }
        //        }
        //    }

        //    return null;
        //}

        public UserDto AddAuthenticatedUser(UserDto user)
        {
            if (!tokenEmailPairs.ContainsKey(user.Key)) {
                tokenEmailPairs.Add(user.Key, user.Email);
            }
            return user;
        }

        public User GetUserByToken(string token) {
            if (token==null || !tokenEmailPairs.ContainsKey(token))
                return null;

            string email = tokenEmailPairs[token];
            if (!registeredUsers.ContainsKey(email))
                return null;
            return registeredUsers[email];
        }
    }
}
