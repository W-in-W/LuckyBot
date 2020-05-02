using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using MongoDB.Bson;

namespace LuckyBot
{
    public class BotCommands
    {
        [Command("find"), Aliases("f"), Description("Поиск оружия по названию.")]
        public async Task ShowWeaponPrice(CommandContext ctx, string weaponName)
        {
            List<BsonDocument> weaponList = await Core.GetWeaponListAsync(weaponName);
            var uniqueWeapons = weaponList.Select(e => e["Weapon"]).Distinct().ToList();
        }
    }
}