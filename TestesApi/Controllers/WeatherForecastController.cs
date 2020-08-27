using Jnz.JaegerExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OpenTracing;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestesApi.Controllers
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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITracer tracer;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory httpClientFactory, ITracer tracer)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            this.tracer = tracer;
        }

        [HttpGet]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client)]
        public async Task<IEnumerable<WeatherForecast>> GetAsync([FromServices] IDistributedCache distributedCache)
        {
            Request.Headers.TryGetValue("Correlation-Token", out var correlationToken);
            tracer.ActiveSpan.SetTag("Correlation-Token", correlationToken.ToString());

            tracer.ActiveSpan.SetBaggageItem("Correlation-Token", correlationToken);
            //var resInCahce = await distributedCache.GetAsync("WeatherForecast");
            //if (resInCahce != null)
            //{
            //    tracer.ActiveSpan?.Log("returning from cache");
            //    return resInCahce.FromByteArray<IEnumerable<WeatherForecast>>();
            //}

            //tracer.ActiveSpan.Log("dados");

            var client = _httpClientFactory.CreateClient("wheather");
            tracer.ActiveSpan?.Log("Calling api");
            tracer.ActiveSpan.SetOperationName("GetAsyncTestesApi");
            var res = await client.GetAsync("http://localhost:5000/weatherforecast");


            if (res.Headers.CacheControl != null)
            {
                _logger.LogInformation(res.Headers.CacheControl.MaxAge.ToString());
            }

            var content = await res.Content.ReadAsStreamAsync();
            var resultado = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(content);

            tracer.ActiveSpan?.Log("Putting in cache");
            await distributedCache.SetAsync("WeatherForecast", resultado.ToByteArray());
            return resultado;
        }
    }
}
