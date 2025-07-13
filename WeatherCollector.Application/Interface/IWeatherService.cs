using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherCollector.Domain;

namespace WeatherCollector.Application.Interface
{
    public interface IWeatherService
    {
         Task<WeatherRecord?> GetWeatherByCityIdAsync(int cityId);
    }
}
