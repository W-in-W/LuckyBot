using System.Collections.Generic;
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
            var collection = database.GetCollection<BsonDocument>("AllWeapons");
            return collection;
        }

        public static async Task<List<BsonDocument>> GetWeaponListAsync(string weaponName)
        {
            var regex = new Regex(weaponName, RegexOptions.IgnoreCase);
            var filter = new BsonDocument("Weapon", regex);
            var weaponList = await GetCollection().Find(filter).ToListAsync();
            return weaponList;
        }
    }
}