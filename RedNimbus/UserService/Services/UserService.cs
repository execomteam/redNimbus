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
using RedNimbus.DTO.Enums;
using UserService.Database;
using MySql.Data.MySqlClient;
using RedNimbus.Either.Mappings;
using UserService.DatabaseModel;
using Microsoft.EntityFrameworkCore;

namespace RedNimbus.UserService.Services
{
    public class UserService : IUserService
    {
        private UserDatabaseUtils userDatabaseUtils;
        public UserService()
        {
            userDatabaseUtils = new UserDatabaseUtils();
        }
        private static readonly Dictionary<string, string>  tokenEmailPairs = new Dictionary<string, string>();

        #region Validation functions

        private bool IsEmailValid(User user)
        {
            return RegexUtilities.IsValidEmail(user.Email);
        }

        private bool IsPasswordValid(User user)
        {
            return Regex.IsMatch(user.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,24}$") && !String.IsNullOrWhiteSpace(user.Password);
        }

        private bool IsFirstNameValid(User user)
        {
            return Regex.IsMatch(user.FirstName, @"^[a-z A-Z]+$");
        }
        private bool IsLastNameValid(User user)
        {
            return Regex.IsMatch(user.LastName, @"^[a-z A-Z]+$");
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
            if (!IsFirstNameValid(user))
            {
                return new Left<IError, User>(new FormatError("Firstname is empty!", ErrorCode.FirstNameNullEmptyOrWhiteSpace));
            }

            if (!IsLastNameValid(user))
            {
                return new Left<IError, User>(new FormatError("Lastname is empty!", ErrorCode.LastNameNullEmptyOrWhiteSpace));
            }

            if (!IsEmailValid(user))
            {
                return new Left<IError, User>(new FormatError("Email format is unacceptable!", ErrorCode.EmailWrongFormat));
            }

            if (!IsPasswordValid(user))
            {
                return new Left<IError, User>(new FormatError("Password format is unacceptable!", ErrorCode.PasswordWrongFormat));
            }

            user.Id = Guid.NewGuid();
            
            user.Password = HashHelper.ComputeHash(user.Password);

            try
            {
                userDatabaseUtils.RegisterUser(user);
            }
            catch (DbUpdateException)
            {
                return new Left<IError, User>(new FormatError("Email already exist", ErrorCode.EmailAlreadyUsed) );
            }
            catch (Exception)
            {
                return new Left<IError, User>(new InternalServisError("ServiceError", ErrorCode.InternalServerError));
            }

            return new Right<IError, User>(user);
        }

        public Either<IError, User> Authenticate(User user)
        {

            if (String.IsNullOrWhiteSpace(user.Email))
            {
                return new AuthenticationError("Password field empty!", ErrorCode.IncorrectEmailOrPassword);
            }

            if (String.IsNullOrWhiteSpace(user.Password)){
                return new AuthenticationError("Password field empty!", ErrorCode.IncorrectEmailOrPassword);
            }


            if (userDatabaseUtils.CheckIfAlreadyRegistered(user.Email))
            {
                var registeredUser = userDatabaseUtils.getUserByEmail(user.Email);
                if (registeredUser.Password == HashHelper.ComputeHash(user.Password))
                {
                    return registeredUser;
                }

            }

            return new AuthenticationError("Email or Password is incorect", ErrorCode.IncorrectEmailOrPassword);

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

        public Either<IError, User> GetUserByToken(string token) {
            if (token == null || !tokenEmailPairs.ContainsKey(token))
            {
                return new NotFoundError("Requested user data not found", ErrorCode.UserNotFound);
            }

            string email = tokenEmailPairs[token];
            if (!userDatabaseUtils.CheckIfAlreadyRegistered(email))
            {
                return new NotFoundError("Requested user data not found", ErrorCode.UserNotRegistrated);
            }

            User registeredUser = userDatabaseUtils.getUserByEmail(email);
            registeredUser.Key = token;

            return registeredUser;
        }
    }
}
