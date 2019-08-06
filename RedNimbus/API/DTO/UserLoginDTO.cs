using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.API.DTO
{
    public class UserLoginDTO
    {
        private string email;
        private string password;

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
    }
}
