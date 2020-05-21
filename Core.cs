using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LuckyBot
{
    public static class Core
    {
        public static IMongoCollection<BsonDocument> GetCollection()
        {
            MongoClient mongoClient = new MongoClient(Program.connectionString);
            IMongoDatabase database = mongoClient.GetDatabase("DayzWeaponDB");
            var collection = database.GetCollection<BsonDocument>("AllWeaponsNew");
            return collection;
        }

        public static async Task<List<BsonDocument>> GetWeaponListAsync(string weaponName)
        {
            var regex = new Regex(weaponName, RegexOptions.IgnoreCase);
            var filter = new BsonDocument("Weapon", regex);
            var weaponList = await GetCollection().Find(filter).ToListAsync();
            foreach (var item in weaponList)
            {
                if (item["Shop"] == "1") item["Shop"] = "Green Mountain и Green Forest";
                else if (item["Shop"] == "2") item["Shop"] = "Altar Black Market";
                else if (item["Shop"] == "3") item["Shop"] = "High Militairy Trader";
                else item["Shop"] = "Неопознано";
            }
            return weaponList;
        }
    }
}