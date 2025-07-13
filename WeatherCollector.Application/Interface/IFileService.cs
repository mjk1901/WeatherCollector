using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherCollector.Domain;

namespace WeatherCollector.Application.Interface
{
    public interface IFileService
    {
        Task<List<(int cityId, string cityName)>> ReadCityListAsync(string filePath);
        Task WriteWeatherToFileAsync(string outputDir, WeatherRecord weather);
    }
}
