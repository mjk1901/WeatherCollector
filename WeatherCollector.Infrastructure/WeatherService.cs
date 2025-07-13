using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using WeatherCollector.Application.Interface;
using WeatherCollector.Common;
using WeatherCollector.Domain;

namespace WeatherCollector.Infrastructure
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
 

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }

        public async Task<WeatherRecord?> GetWeatherByCityIdAsync(int cityId)
        {
            string requestUrl = ConfigHelper.BaseUrl + "?id=" + cityId + "&appid=" + ConfigHelper.AppKey + "&units=metric";
            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var reason = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to retrieve weather data due to Invalid API Key.");
                }
                else
                {
                    var reason = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to retrieve weather data for City ID {cityId}.");
                }

            }
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            return new WeatherRecord
            {
                CityId = cityId,
                CityName = json.GetProperty("name").GetString(),
                Temperature = json.GetProperty("main").GetProperty("temp").GetDouble(),
                WeatherDescription = json.GetProperty("weather")[0].GetProperty("description").GetString(),
                RetrievedAt = DateTime.UtcNow
            };




        }
    }
}
