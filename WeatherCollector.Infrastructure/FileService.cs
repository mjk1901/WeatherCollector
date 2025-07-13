using System.Text.Json;
using WeatherCollector.Application.Interface;
using WeatherCollector.Domain;

namespace WeatherCollector.Infrastructure
{
    public class FileService : IFileService
    {
        public async Task<List<(int cityId, string cityName)>> ReadCityListAsync(string filePath)
        {
            var lines = await File.ReadAllLinesAsync(filePath);
            return lines
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line =>
                {
                    var parts = line.Split('=');
                    return (int.Parse(parts[0]), parts[1]);
                }).ToList();
        }

        public async Task WriteWeatherToFileAsync(string outputDir, WeatherRecord weather)
        {
            var filePath = Path.Combine(outputDir, $"{weather.CityName}_{DateTime.UtcNow:yyyy-MM-dd_hh-mm-ss}.json");
            var json = JsonSerializer.Serialize(weather, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
        }
    }
}
