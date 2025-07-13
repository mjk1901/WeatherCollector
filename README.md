# WeatherCollector

# 🌦️ Weather Data Collection Service

A .NET Core Web API that retrieves and stores daily weather data for global cities using the OpenWeather API.

---

## 📌 Features

- Reads list of City IDs from `cities.txt`
- Fetches daily weather via OpenWeather API
- Saves results as daily JSON files (one per city per day)
- Maintains historical records
- Built with Clean Architecture, SOLID principles, and TDD

---

## ⚙️ Technologies

- .NET 8 Web API
- BackgroundService for daily job
- HttpClient with retry policy (Polly)
- JSON file persistence
- Unit tests (xUnit + Moq)
- Config via `appsettings.json` or secrets

---

## 🚀 Run Locally

### Prerequisites
- .NET 8 SDK
- OpenWeather API Key: [https://openweathermap.org/api](https://openweathermap.org/api)

### Setup

```bash
cd src/WeatherCollector.Api
dotnet user-secrets init
dotnet user-secrets set OpenWeather:AppId "your-api-key"
dotnet run
