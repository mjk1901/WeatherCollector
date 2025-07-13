using WeatherJobBackgroundService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<WeatherJobService>();

var host = builder.Build();
host.Run();
