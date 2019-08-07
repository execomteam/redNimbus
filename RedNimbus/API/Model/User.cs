using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.API.Model
{
   
    public class User
    {
        private string firstName;
        private string lastName;
        private string email;
        private string password;

        public User(string _firstName, string _lastName, string _email, string _password)
        {
            firstName = _firstName;
            lastName = _lastName;
            email = _email;
            password = _password;
        }

        #region properties

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        #endregion
    }
}
