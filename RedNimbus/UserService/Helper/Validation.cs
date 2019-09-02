using RedNimbus.UserService.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace UserService.Helper
{
    public static class Validation
    {
        public static bool IsFirstNameValid(string firstName)
        {
            return Regex.IsMatch(firstName, @"^[a-z A-Z]+$");
        }

        public static bool IsLastNameValid(string lastName)
        {
            return Regex.IsMatch(lastName, @"^[a-z A-Z]+$");
        }

        public static bool IsEmailValid(string email)
        {
            return RegexUtilities.IsValidEmail(email);
        }

        public static bool IsPasswordValid(string password)
        {
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,24}$") && !String.IsNullOrWhiteSpace(password);
        }

        public static bool IsPhoneValid(string phoneNumber)
        {
            if (!String.IsNullOrWhiteSpace(phoneNumber))
                return Regex.IsMatch(phoneNumber, @"^[0-9()-]+$");

            return true;
        }
    }
}
