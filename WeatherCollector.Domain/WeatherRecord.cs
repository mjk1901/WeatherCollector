namespace WeatherCollector.Domain
{
    public class WeatherRecord
    {
        public string CityName { get; set; } = string.Empty;
        public int CityId { get; set; }
        public DateTime RetrievedAt { get; set; }
        public double Temperature { get; set; }
        public string WeatherDescription { get; set; } = string.Empty;
    }
}
