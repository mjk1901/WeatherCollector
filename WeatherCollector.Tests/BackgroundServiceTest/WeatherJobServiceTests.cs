using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherCollector.Application.Interface;
using WeatherCollector.Common;
using WeatherCollector.Domain;
using WeatherCollector.Infrastructure.BackgroundServices;
using Xunit;

public class WeatherJobServiceTests
{
    private static (WeatherJobService sut,
                   Mock<IWeatherService> weatherMock,
                   Mock<IFileService> fileMock,
                   Mock<ILogger<WeatherJobService>> logMock,
                   CancellationTokenSource cts)
        BuildSutWithMocks()
    {
        // ─── Arrange common mocks ────────────────────────────────────────────────
        var weatherMock = new Mock<IWeatherService>(MockBehavior.Strict);
        var fileMock = new Mock<IFileService>(MockBehavior.Strict);
        var logMock = new Mock<ILogger<WeatherJobService>>();

        // Provide fake DI container
        var provider = new ServiceCollection()
            .AddSingleton(weatherMock.Object)
            .AddSingleton(fileMock.Object)
            .BuildServiceProvider();

        var sut = new WeatherJobService(provider, logMock.Object);
        var cts = new CancellationTokenSource();

        // Cancel after one loop so ExecuteAsync will finish
        cts.CancelAfter(500);      // 0.5 s is plenty

        return (sut, weatherMock, fileMock, logMock, cts);
    }

    [Fact]
    public async Task ExecuteAsync_Processes_All_Cities_And_Writes_Output()
    {
        // ─── Arrange ─────────────────────────────────────────────────────────────
        var (sut, weatherMock, fileMock, logMock, cts) = BuildSutWithMocks();

        var cities = new List<(int cityId, string cityName)>
        {
            (2643743, "London"),
            (5128581, "New York")
        };

        fileMock.Setup(f => f.ReadCityListAsync(ConfigHelper.InputFile))
                .ReturnsAsync(cities);

        // Return dummy weather object for every city
        weatherMock.Setup(w => w.GetWeatherByCityIdAsync(It.IsAny<int>()))
                   .ReturnsAsync(new WeatherRecord());      // WeatherDto = whatever your real DTO is

        fileMock.Setup(f => f.WriteWeatherToFileAsync(ConfigHelper.OutputFolder,
                                                      It.IsAny<WeatherRecord>()))
                .Returns(Task.CompletedTask);

        // ─── Act ────────────────────────────────────────────────────────────────
        await sut.StartAsync(cts.Token);   // Runs ExecuteAsync under the hood

        // ─── Assert ─────────────────────────────────────────────────────────────
        fileMock.Verify(f => f.ReadCityListAsync(ConfigHelper.InputFile), Times.Once);

        // Exactly once per city
        weatherMock.Verify(w => w.GetWeatherByCityIdAsync(2643743), Times.Once);
        weatherMock.Verify(w => w.GetWeatherByCityIdAsync(5128581), Times.Once);
        fileMock.Verify(f => f.WriteWeatherToFileAsync(ConfigHelper.OutputFolder,
                                                       It.IsAny<WeatherRecord>()),
                        Times.Exactly(cities.Count));

        // Information log was written
        logMock.VerifyLog(LogLevel.Information, "Weather job executed.", Times.AtLeastOnce());
    }

    [Fact]
    public async Task ExecuteAsync_When_Exception_Is_Thrown_Logs_Error()
    {
        // ─── Arrange ─────────────────────────────────────────────────────────────
        var (sut, weatherMock, fileMock, logMock, cts) = BuildSutWithMocks();

        fileMock.Setup(f => f.ReadCityListAsync(It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Busted"));

        // ─── Act ────────────────────────────────────────────────────────────────
        await sut.StartAsync(cts.Token);

        // ─── Assert ─────────────────────────────────────────────────────────────
        logMock.VerifyLog(LogLevel.Error, "Weather job failed.", Times.AtLeastOnce());
    }
}

/// <summary>
/// Small helper so we don't repeat the verbose ILogger.Verify pattern everywhere.
/// </summary>
internal static class LoggerMoqExtensions
{
    public static void VerifyLog<T>(this Mock<ILogger<T>> logger,
                                    LogLevel level,
                                    string contains,
                                    Times times)
    {
        logger.Verify(x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(contains)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            times);
    }
}
