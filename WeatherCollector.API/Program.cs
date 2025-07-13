using WeatherCollector.Application.Interface;
using WeatherCollector.Common;
using WeatherCollector.Infrastructure;
using WeatherCollector.Infrastructure.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
ConfigHelper.Initialize(builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddHostedService<WeatherJobService>();

var app = builder.Build();
//ConfigItems.Configuration = app.Configuration;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();