﻿using System.ComponentModel.DataAnnotations;

namespace RedNimbus.API.DTO
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
    }
}