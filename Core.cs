using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LuckyBot
{
    public static class Core
    {
        private static IMongoCollection<BsonDocument> _collection;
        public static IMongoCollection<BsonDocument> GetCollection()
        {
            if (_collection != null) return _collection;
            MongoClient mongoClient = new MongoClient(Program.connectionString);
            IMongoDatabase database = mongoClient.GetDatabase("DayzWeaponDB");
            _collection = database.GetCollection<BsonDocument>("AllWeaponsNew");
            return _collection;
        }

        public static async Task<List<BsonDocument>> GetWeaponListAsync(string weaponName)
        {
            var regex = new Regex(weaponName, RegexOptions.IgnoreCase);
            var filter = new BsonDocument("Weapon", regex);
            var weaponList = await Program.collection.Find(filter).ToListAsync();
            foreach (var item in weaponList)
            {
                switch (item["Shop"].AsString)
                {
                    case "1":
                        item["Shop"] = ":mountain: **Green Mountain** и :evergreen_tree: **Green Forest**";
                        break;
                    case "2":
                        item["Shop"] = ":pirate_flag: **Altar Black Market**";
                        break;
                    case "3":
                        item["Shop"] = ":moneybag: **High Militairy Trader**";
                        break;
                    default:
                        item["Shop"] = "Неопознано";
                        break;
                }
            }
            return weaponList;
        }
    }
}