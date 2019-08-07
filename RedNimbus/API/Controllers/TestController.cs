using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpPost("[action]")]
        public string Testiraj(string one, string two)
        {
            string str = one + " " + two;
            Console.WriteLine(str);
            return str;
        }

        [HttpGet("[action]")]
        public string Plain()
        {
            return "Caooo";
        }

    }
}