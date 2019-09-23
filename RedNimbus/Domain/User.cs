using System;

namespace RedNimbus.Domain
{
    public class User
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string Key { get; set; }

        public override string ToString()
        {
            return String.Format("first name = {0}; last name = {1}; email = {2}; password = {3}; phone number = {4}; active account: {5}",
                    this.FirstName,
                    this.LastName,
                    this.Email,
                    this.Password,
                    this.PhoneNumber,
                    this.ActiveAccount);
        }
      
        public bool ActiveAccount { get; set; }
    }
}
