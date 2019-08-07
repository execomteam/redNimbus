using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.API.DTO
{
    public class UserDTO
    {
        public UserDTO(string _firstName, string _lastName, string _email, string _key)
        {
            FirstName = _firstName;
            LastName = _lastName;
            Email = _email;
            Key = _key;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Key { get; set; }
    }
}
