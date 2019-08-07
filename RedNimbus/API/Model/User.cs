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
        private string secondName;
        private string email;
        private string password;

        public User(string fn, string sn, string e, string p)
        {
            firstName = fn;
            secondName = sn;
            email = e;
            password = p;
        }

        #region properties
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string SecondName
        {
            get { return secondName; }
            set { secondName = value; }
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
