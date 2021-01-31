using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockcore.DataVault.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("identity")]
    public class IdentityController
    {
        private readonly ILogger<IdentityController> log;

        public IdentityController(ILogger<IdentityController> logger)
        {
            log = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
            })

            .ToArray();
        }

    }
}
