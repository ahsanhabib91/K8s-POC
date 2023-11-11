using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace UserService.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IDatabase _redis;
    private readonly IServer _redisServer;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,
        IConnectionMultiplexer connectionMultiplexer,
        IConfiguration configuration)
    {
        _logger = logger;
        _redis = connectionMultiplexer.GetDatabase();
        _redisServer = connectionMultiplexer.GetServer(configuration.GetConnectionString("Redis"));
    }

    private string GetKey(string id) => $"user-service-{id}";

    [HttpPost]
    public async Task<WeatherForecast> Save()
    {
        var id = Guid.NewGuid();
        var key = GetKey(id.ToString());
        var weather = new WeatherForecast
        {
            Id = id,
            Date = DateTime.Now,
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        };

        await _redis.StringSetAsync(key, JsonSerializer.Serialize(weather));
        return weather;
    }

    [HttpGet("{id}")]
    public async Task<WeatherForecast?> Get(string id)
    {
        var value = await _redis.StringGetAsync(GetKey(id));
        byte[] value2 = await _redis.StringGetAsync(GetKey(id));

        if (!value.HasValue) return new WeatherForecast();

        var weather = JsonSerializer.Deserialize<WeatherForecast>(value);
        return weather;
    }

    [HttpGet("id/all")]
    public List<string> GetAllIds()
    {
        return _redisServer.Keys(pattern: "*").Select(key => (string)key).ToList();
    }

}