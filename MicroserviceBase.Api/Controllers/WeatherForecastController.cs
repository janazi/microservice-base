using MassTransit;
using MicroserviceBase.Api.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTracing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MicroserviceBase.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IPublishEndpoint publishEndpoint;
        private readonly ISendEndpointProvider sendEndpointProvider;
        private readonly ITracer tracer;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger
            , IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider, ITracer tracer)
        {
            _logger = logger;
            this.publishEndpoint = publishEndpoint;
            this.sendEndpointProvider = sendEndpointProvider;
            this.tracer = tracer;
        }

        [HttpGet]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetAsync()
        {
            //await publishEndpoint.Publish<StartDataPreparationCommand>(new { MigrationId = Guid.NewGuid() });
            tracer.ActiveSpan.SetOperationName("GetAsyncBaseApi");

            await sendEndpointProvider.Send<StartDataPreparationCommand>(new { MigrationId = Guid.NewGuid() });
            _logger.LogInformation("teste", "vai");
            var rng = new Random();
            var dados = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            return new OkObjectResult(dados);
        }
    }
}
