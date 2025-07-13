using Microsoft.AspNetCore.Mvc;
using WeatherCollector.Application.Interface;
using WeatherCollector.Common;

namespace WeatherCollector.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly IFileService _fileService;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController( IWeatherService weatherService, IFileService fileService, ILogger<WeatherForecastController> logger )
    {
        _weatherService = weatherService;
        _fileService = fileService;
        _logger = logger;
    }
    int ProcessedCount = 0;

    [HttpPost("collect")]
    public async Task<IActionResult> CollectWeatherData()
    {
        try
        {
            _logger.LogInformation("Weather collection started!");
           
            var cityList = await _fileService.ReadCityListAsync(ConfigHelper.InputFile!);

            foreach (var (cityId, _) in cityList)
            {
                var weather = await _weatherService.GetWeatherByCityIdAsync(cityId);
                await _fileService.WriteWeatherToFileAsync(ConfigHelper.OutputFolder!, weather);
                ProcessedCount++;
            }
            return Ok($"Weather data processed for {ProcessedCount} cities.");
        }
  
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            if (ex.Message.Contains("Failed to retrieve"))
            {
                return BadRequest(new { message = ex.Message });
            }
            else
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }       
    }

}
