using WeatherCollector.Application.Interface;
using WeatherCollector.Common;

namespace WeatherJobBackgroundService
{
    public class WeatherJobService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<WeatherJobService> _logger;

        public WeatherJobService(IServiceProvider services, ILogger<WeatherJobService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var weatherService = scope.ServiceProvider.GetRequiredService<IWeatherService>();
                var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();

                try
                {
                    var cities = await fileService.ReadCityListAsync(ConfigHelper.InputFile);
                    foreach (var (id, _) in cities)
                    {
                        var weather = await weatherService.GetWeatherByCityIdAsync(id);
                        await fileService.WriteWeatherToFileAsync(weather);
                    }
                    _logger.LogInformation("Weather data collected successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error collecting weather data.");
                }

                // Wait until same time tomorrow
                var nextRun = DateTime.UtcNow.Date.AddDays(1).AddHours(1); // runs every day at 01:00 UTC
                var delay = nextRun - DateTime.UtcNow;
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
