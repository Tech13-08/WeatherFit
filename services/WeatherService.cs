using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WeatherFit.Services
{
    public class WeatherService
    {
        private static readonly HttpClient client = new HttpClient();
        private const string ApiKey = "6e9874da1d2d948513b31d020376a7d0";

        public async Task<string> GetWeatherDataAsync(string lat, string lon)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={ApiKey}&units=metric";

            try
            {
                var response = await client.GetStringAsync(url);
                return response;
            }
            catch (Exception)
            {
                return "Error fetching weather data.";
            }
        }
    }
}
