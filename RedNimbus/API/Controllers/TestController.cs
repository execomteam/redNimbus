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
        [HttpGet("[action]")]

        public string Testiraj(string one, string two)
        {
            return one + two;
        }

        [HttpGet("[action]")]
        public string Plain()
        {
            return "Caooo";
        }

    }
}