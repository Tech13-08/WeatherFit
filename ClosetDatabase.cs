using LiteDB;
using System.Collections.Generic;
using WeatherFit.Models;

namespace WeatherFit
{
    public class ClosetDatabase
    {
        private readonly string _databasePath = "ClosetData.db";

        public void AddCloset(Closet closet)
        {
            using var db = new LiteDatabase(_databasePath);
            var closets = db.GetCollection<Closet>("closets");
            closets.Insert(closet);
        }

        public List<Closet> GetClosets()
        {
            using var db = new LiteDatabase(_databasePath);
            return db.GetCollection<Closet>("closets").FindAll().ToList();
        }

        public void UpdateCloset(Closet closet)
        {
            using var db = new LiteDatabase(_databasePath);
            var closets = db.GetCollection<Closet>("closets");
            closets.Update(closet);
        }

        public void DeleteCloset(Guid closetId)
        {
            using var db = new LiteDatabase(_databasePath);
            var closets = db.GetCollection<Closet>("closets");
            var clothingItems = db.GetCollection<ClothingItem>("clothingItems");
            clothingItems.DeleteMany(item => item.ClosetId == closetId.ToString());
            closets.DeleteMany(item => item.Id == closetId);
        }

        public void DeleteClothingItem(ClothingItem clothingItem)
        {
            using var db = new LiteDatabase(_databasePath);
            var clothingItems = db.GetCollection<ClothingItem>("clothingItems");
            clothingItems.DeleteMany(item => item.Id == clothingItem.Id);
        }
    }
}
