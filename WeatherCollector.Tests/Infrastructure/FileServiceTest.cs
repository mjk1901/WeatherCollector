using System.Text.Json;
using FluentAssertions;
using WeatherCollector.Application;
using WeatherCollector.Domain;
using WeatherCollector.Infrastructure;
using Xunit;
namespace WeatherCollector.Tests.Infrastructure
{
    public class FileServiceTest
    {
        [Fact]
        public async Task ReadCityIdsAsync_Should_Return_CityIds()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            await File.WriteAllLinesAsync(tempFile, new[]
            {
                    "2643741=City of London",
                    "2988507=Paris",
                    "1273294=Delhi"
            });
            var fileService = new FileService();

            // Act
            var result = await fileService.ReadCityListAsync(tempFile);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new List<(int, string)>
            {
                (2643741, "City of London"),
                (2988507, "Paris"),
                (1273294, "Delhi")
            });

            File.Delete(tempFile);
        }

        [Fact]
        public async Task WriteCityWeatherAsync_Should_Write_File_To_OutputDirectory()
        {
            // Arrange
            var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(outputDir);

            var fileService = new FileService();

            var weather = new WeatherRecord
            {
                CityName = "Delhi",
                Temperature = 30.5,
                WeatherDescription = "clear sky"
            };

            // Act
            await fileService.WriteWeatherToFileAsync(outputDir, weather);

            // Assert
            var files = Directory.GetFiles(outputDir);
            files.Should().HaveCount(1);

            var content = await File.ReadAllTextAsync(files[0]);
            var deserialized = JsonSerializer.Deserialize<WeatherRecord>(content);

            deserialized.Should().NotBeNull();
            deserialized!.CityName.Should().Be("Delhi");
            deserialized.Temperature.Should().Be(30.5);
            deserialized.WeatherDescription.Should().Be("clear sky");

            Directory.Delete(outputDir, true);
        }
    }
}
