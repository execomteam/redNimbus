using System;
using System.ComponentModel.DataAnnotations;

namespace RedNimbus.DTO
{
    public class CreateUserDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public override string ToString()
        {
            return String.Format("first name = {0}; last name = {1}; email = {2}; password = {3}; phone number = {4}",
                    this.FirstName,
                    this.LastName,
                    this.Email,
                    this.Password,
                    this.PhoneNumber);
        }
    }
}
