using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using RedNimbus.Domain;
using RedNimbus.RestUserService.Services.Interfaces;
using RedNimbus.RestUserService.Helper;
using RedNimbus.Either.Errors;
using RedNimbus.Either;
using RedNimbus.DTO;
using RedNimbus.DTO.Enums;
using UserService.Database;
using MySql.Data.MySqlClient;
using RedNimbus.Either.Mappings;
using UserService.DatabaseModel;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using RedNimbus.Either.Enums;

namespace RedNimbus.RestUserService.Services
{
    public class UserService : IUserService
    {
        private IMapper _mapper;
        private UserRepository userDatabaseUtils;
        public UserService(IMapper mapper)
        {
            this._mapper = mapper;
            userDatabaseUtils = new UserRepository(mapper);
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
                return new Left<IError, User>(new FormatError("Password must have at least 8 characters, one uppercase letter, one lowcase letter, one number and one special character!", ErrorCode.PasswordWrongFormat));
            }

            user.Id = Guid.NewGuid();
            
            user.Password = HashHelper.ComputeHash(user.Password);

            try
            {
                userDatabaseUtils.SaveUser(user);
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


            if (userDatabaseUtils.CheckIfExists(user.Email))
            {
                var registeredUser = userDatabaseUtils.GetUserByEmail(user.Email);

                if(registeredUser == null)
                {
                    return new AuthenticationError("Account has been deactivated", ErrorCode.AccountDeactivated);
                }
                if (registeredUser.Password == HashHelper.ComputeHash(user.Password))
                {
                    return registeredUser;
                }

            }

            return new AuthenticationError("Email or Password is incorect", ErrorCode.IncorrectEmailOrPassword);

        }

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
            if (!userDatabaseUtils.CheckIfExists(email))
            {
                return new NotFoundError("Requested user data not found", ErrorCode.UserNotRegistrated);
            }

            User registeredUser = userDatabaseUtils.GetUserByEmail(email);
            registeredUser.Key = token;

            return registeredUser;
        }
    }
}
