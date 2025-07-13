using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherCollector.Application.Interface;
using WeatherCollector.Common;

namespace WeatherCollector.Infrastructure.BackgroundServices
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
                    foreach (var (cityId, _) in cities)
                    {
                        var weather = await weatherService.GetWeatherByCityIdAsync(cityId);
                        await fileService.WriteWeatherToFileAsync(ConfigHelper.OutputFolder,weather);
                    }

                    _logger.LogInformation("Weather job executed.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Weather job failed.");
                }

                // Wait foro 24 hours for next run
                var nextRun = DateTime.UtcNow.AddHours(24);
                var delay = nextRun - DateTime.UtcNow;
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
