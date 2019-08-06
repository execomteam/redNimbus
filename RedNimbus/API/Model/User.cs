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
        private string username;
        private string email;
        private string password;

        public User(string fn, string sn, string un, string e, string p)
        {
            firstName = fn;
            secondName = sn;
            username = un;
            email = e;
            password = p;
        }

        #region properties
        [Required]
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        [Required]
        public string SecondName
        {
            get { return secondName; }
            set { secondName = value; }
        }

        [Required]
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        [Required]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [Required]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        #endregion

    }
}
