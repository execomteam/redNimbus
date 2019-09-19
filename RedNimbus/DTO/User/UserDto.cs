using System;

namespace RedNimbus.DTO
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Key { get; set; }

        public override string ToString()
        {
            return String.Format("FirstName = {0}; LastName = {1}; Email = {2}; Token = {3}", FirstName, LastName, Email, Key);
        }
    }
}
