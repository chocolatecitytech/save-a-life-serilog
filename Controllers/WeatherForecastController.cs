using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace working_with_serilog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet]
        [Route("{key}/{loggingLevel}")]
        public IActionResult GetByKey(int key, int loggingLevel)
        {



            if (loggingLevel > -1)
            {
                switch (loggingLevel)
                {
                    case 0:
                        _logger.LogTrace("WeatherForecastController - Trace");
                        break;
                    case 1:
                        _logger.LogDebug("WeatherForecastController - Debug");
                        break;
                    case 2:
                        _logger.LogInformation("WeatherForecastController - Information");
                        break;
                    case 3:
                        _logger.LogWarning("WeatherForecastController - Warning");
                        break;
                    case 4:
                        _logger.LogError(new Exception("Some exception here."), "WeatherForecastController - Error");
                        break;
                    default:
                        _logger.LogCritical("WeatherForecastController - Yikes");
                        break;
                }
            }
            else
            {
                _logger.LogWarning("User did not pass a valid LogLevel: {level}", loggingLevel);
            }
            if (key < 0)
            {
                _logger.LogInformation("index must be greather than -1");
                return BadRequest();
            }
            if (key > Summaries.Length)
            {
                _logger.LogInformation("index cannot be greather than {len}", Summaries.Length);
                return BadRequest();
            }
            try
            {
                var forecast = new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(key),
                    TemperatureC = 55,
                    Summary = Summaries[key]
                };
                _logger.LogWarning("We found this structured forecast: {@forecast} using provided key: {key}", forecast, key);
                return Ok(forecast);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return BadRequest();
            }
        }
    }
}
