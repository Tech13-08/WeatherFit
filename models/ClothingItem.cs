namespace WeatherFit.Models
{
    public class ClothingItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Description { get; set; }
        public bool InLaundry { get; set; }
        public string ClosetId { get; set; }
    }
}
