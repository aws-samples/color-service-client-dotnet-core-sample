using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ServiceA.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ColorController : ControllerBase
    {
        // GET: api/<ColorController>
        [HttpGet]
        public string Get()
        {
            return "Green";
        }

        // GET api/<ColorController>/5
        [HttpGet("{color}")]
        public string Get(string color)
        {
            return color;
        }
    }
}
