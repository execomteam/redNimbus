﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RedNimbus.API.DTO
{
    public class AuthorizeUserDto
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
