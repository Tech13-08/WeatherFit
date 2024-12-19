using LiteDB;

namespace WeatherFit.Models
{
    public class Closet
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
        public List<ClothingItem> ClothingItems { get; set; } = new List<ClothingItem>();

        public override string ToString() => Name;
    }
}
