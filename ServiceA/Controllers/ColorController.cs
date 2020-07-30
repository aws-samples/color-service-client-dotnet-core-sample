﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiceA.Models;
using ServiceA.Utility;

namespace ServiceA.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ColorController : ControllerBase
    {
        public IOptionsSnapshot<Settings> Settings { get; }

        public ColorController(IOptionsSnapshot<Settings> settings)
        {
            this.Settings = settings;
        }

        // GET: api/<ColorController>
        [HttpGet]
        public string Get() => this.Settings.Value.Color.OrDefaultIfBlank("Green");

        // GET api/<ColorController>/Red
        [HttpGet("{color}")]
        public string Get(string color) => color.OrDefaultIfBlank(this.Get());
    }
}
