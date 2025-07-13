using Microsoft.Extensions.Configuration;

public static class ConfigHelperTestInitializer
{
    public static void Initialize()
    {
        var configData = new Dictionary<string, string?>
        {
            { "Data:Paths:InputFile", "dummy-input.txt" },
            { "Data:Paths:OutputFolder", "dummy-output" },
            { "Data:OpenWeather:BaseUrl", "https://api.test.com" },
            { "Data:OpenWeather:AppId", "dummy-api-key" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        WeatherCollector.Common.ConfigHelper.Configuration = configuration;
    }
}