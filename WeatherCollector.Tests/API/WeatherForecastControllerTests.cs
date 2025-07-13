using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherCollector.API.Controllers;
using WeatherCollector.Application.Interface;
using WeatherCollector.Domain;
using Xunit;


namespace WeatherCollector.Tests.API
{
    public class WeatherForecastControllerTests
    {
        private readonly Mock<IWeatherService> _weatherServiceMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly Mock<ILogger<WeatherForecastController>> _loggerMock;
        private readonly WeatherForecastController _controller;

        public WeatherForecastControllerTests()
        {
            ConfigHelperTestInitializer.Initialize();

            _weatherServiceMock = new Mock<IWeatherService>();
            _fileServiceMock = new Mock<IFileService>();
            _loggerMock = new Mock<ILogger<WeatherForecastController>>();

            _controller = new WeatherForecastController(
                _weatherServiceMock.Object,
                _fileServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ProcessWeatherAsync_ReturnsOk_WhenProcessingIsSuccessful()
        {
            // Arrange
            var cityList = new List<(int, string)>
            {
                (2643741, "London"),
                (2988507, "Paris")
            };

            var weatherLondon = new WeatherRecord
            {
                CityName = "London",
                Temperature = 20,
                WeatherDescription = "cloudy"
            };

            var weatherParis = new WeatherRecord
            {
                CityName = "Paris",
                Temperature = 25,
                WeatherDescription = "sunny"
            };

            _fileServiceMock.Setup(s => s.ReadCityListAsync(It.IsAny<string>()))
                .ReturnsAsync(cityList);

            _weatherServiceMock.Setup(s => s.GetWeatherByCityIdAsync(2643741))
                .ReturnsAsync(weatherLondon);

            _weatherServiceMock.Setup(s => s.GetWeatherByCityIdAsync(2988507))
                .ReturnsAsync(weatherParis);

            _fileServiceMock.Setup(s => s.WriteWeatherToFileAsync(It.IsAny<string>(), It.IsAny<WeatherRecord>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CollectWeatherData();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().Be("Weather data processed for 2 cities.");
        }
    }
}
