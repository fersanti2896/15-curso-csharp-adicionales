using Entidades;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
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

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get() {
            HttpContext.Response.Headers.Add("Ejemplo-Respuesta", "Valor defecto");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public ActionResult<int> Post(WeatherForecast weatherForecast) {
            Console.WriteLine(weatherForecast.Summary);

            return 1;
        }

        [HttpGet("mayusculas")]
        public ActionResult<string> GetMayus([FromHeader] string? valor = "Soy Fernando") {
            Console.WriteLine(valor);

            return valor!.ToUpper();
        }
    }
}