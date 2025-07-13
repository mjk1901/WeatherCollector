using Microsoft.Extensions.Configuration;

namespace WeatherCollector.Common
{
    public static class ConfigHelper
    {
        public static IConfiguration Configuration { get; set; } = null!;

        public static void Initialize(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static string BaseUrl =>
            Configuration["Data:OpenWeather:BaseUrl"] ?? throw new InvalidOperationException("BaseUrl not found");

        public static string AppKey =>
            Configuration["Data:OpenWeather:AppId"] ?? throw new InvalidOperationException("AppId not found");

        public static string InputFile =>
            Configuration["Data:Paths:InputFile"] ?? throw new InvalidOperationException("InputFile not found");

        public static string OutputFolder =>
            Configuration["Data:Paths:OutputFolder"] ?? throw new InvalidOperationException("OutputFolder not found");
    }
}

