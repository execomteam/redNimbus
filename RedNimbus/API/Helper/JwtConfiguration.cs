using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helper
{
    public class JwtConfiguration
    {
        public string Issuer { get; set; }
        public string Key { get; set; }
    }
}
