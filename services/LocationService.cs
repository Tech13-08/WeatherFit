using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherFit.Models;

namespace WeatherFit.Services
{
    public class LocationService
    {
        private static readonly HttpClient client = new HttpClient();
        private const string ApiKey = "d6361cd9625e4ef498cbd521097d5da6";

        public async Task<List<LocationData>> GetCitySuggestionsAsync(string query)
        {
            string url = $"https://api.opencagedata.com/geocode/v1/json?q={query}&key={ApiKey}&limit=5";

            try
            {
                var response = await client.GetStringAsync(url);
                return ParseSuggestions(response);
            }
            catch
            {
                return new List<LocationData>();
            }
        }

        private List<LocationData> ParseSuggestions(string jsonResponse)
        {
            var suggestions = new List<LocationData>();
            using JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement results = doc.RootElement.GetProperty("results");

            foreach (JsonElement result in results.EnumerateArray())
            {
                string displayName = result.GetProperty("formatted").GetString();
                string lat = result.GetProperty("geometry").GetProperty("lat").ToString();
                string lon = result.GetProperty("geometry").GetProperty("lng").ToString();

                suggestions.Add(new LocationData(displayName, lat, lon));
            }
            return suggestions;
        }
    }
}
