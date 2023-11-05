using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private HttpClient HttpClient => _httpClientFactory.CreateClient("user-service");

    public UserController(ILogger<UserController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("weather-forecast")]
    public async Task<ActionResult<WeatherForecast>> Save()
    {
        var response = await HttpClient.PostAsync("WeatherForecast", null);
        var weather = await response.Content.ReadFromJsonAsync<WeatherForecast>();
        return weather ?? new WeatherForecast();
    }

    [HttpGet("weather-forecast/{id}")]
    public async Task<ActionResult<WeatherForecast>> Get(string id)
    {
        try
        {
            var response = await HttpClient.GetAsync($"WeatherForecast/{id}");

            if (!response.IsSuccessStatusCode) return new WeatherForecast();

            var weather = await response.Content.ReadFromJsonAsync<WeatherForecast>() ?? new WeatherForecast();
            return weather;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

}