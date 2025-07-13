using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using WeatherCollector.Application.Interface;
using WeatherCollector.Domain;
using WeatherCollector.Infrastructure;
using Microsoft.Extensions.Configuration;
using Xunit;
using WeatherCollector.Common;

namespace WeatherCollector.Tests.Infrastructure
{
    public class WeatherServiceTest
    {
       

        public WeatherServiceTest()
        {
            ConfigHelperTestInitializer.Initialize();
        }


        [Fact]
        public async Task GetWeatherByCityIdAsync_Returns_CityWeather_When_Response_Is_Successful()
        {
            // Arrange
            var cityId = 12345;
            var sampleJson =
    @"{
        ""name"":""London"",
        ""main"":{""temp"":22.5},
        ""weather"":[{""description"":""clear sky""}]
      }";
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(sampleJson),
               });

            var httpClient = new HttpClient(handlerMock.Object);

            var service = new WeatherService(httpClient);

            // Act
            var result = await service.GetWeatherByCityIdAsync(cityId);

            // Assert
            result.Should().NotBeNull();
            result.CityName.Should().Be("London");
            result.Temperature.Should().Be(22.5);
        }

        [Fact]
        public async Task GetWeatherByCityIdAsync_Throws_When_Response_Is_Failure()
        {
            // Arrange
            var cityId = 99999;

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.NotFound
               });

            var httpClient = new HttpClient(handlerMock.Object);
            var service = new WeatherService(httpClient);

            // Act
            Func<Task> act = async () => await service.GetWeatherByCityIdAsync(cityId);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*API returned NotFound*");

        }
    }
}

