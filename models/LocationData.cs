namespace WeatherFit.Models
{
    public class LocationData
    {
        public string DisplayName { get; }
        public string Latitude { get; }
        public string Longitude { get; }

        public LocationData(string displayName, string latitude, string longitude)
        {
            DisplayName = displayName;
            Latitude = latitude;
            Longitude = longitude;
        }

        public override string ToString() => DisplayName;
    }
}
