using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using MongoDB.Bson;

namespace LuckyBot
{
    public class BotCommands
    {
        [Command("find"), Aliases("f"), Description("Поиск оружия по названию.")]
        public async Task ShowWeaponPrice(CommandContext ctx, string weaponName)
        {
            await ctx.TriggerTypingAsync();
            var interactivity = ctx.Client.GetInteractivityModule();
            List<BsonDocument> weaponList = await Core.GetWeaponListAsync(weaponName);
            List<string> uniqueWeapons = weaponList.Select(e => e["Weapon"].AsString).Distinct().ToList();
            uniqueWeapons.Sort();
            if (uniqueWeapons.Count > 9)
            {
                await ctx.RespondAsync("Найдено слишком много совпадений, пожалуйста, уточните запрос.");
            }
            else if (uniqueWeapons.Count > 1)
            {
                string message = "Найдено несколько совпадений, выберите нужное:\n";
                for (int i = 0; i < uniqueWeapons.Count; i++)
                {
                    message += $"{i + 1}: {uniqueWeapons[i]}\n";
                }
                await ctx.RespondAsync(message);
                int userChoice = 0;
                var msg = await interactivity.WaitForMessageAsync(
                    a => a.Author.Id == ctx.User.Id && Int32.TryParse(a.Content, out userChoice) && userChoice > 0 &&
                         userChoice <= uniqueWeapons.Count,
                    TimeSpan.FromSeconds(30));
                if (msg != null)
                {
                    await ctx.TriggerTypingAsync();
                    weaponList = weaponList.Where(e => e["Weapon"] == uniqueWeapons[userChoice - 1]).ToList();
                    message = default;
                    foreach (var item in weaponList)
                    {
                        message += $"{item["Weapon"]} продаётся в {item["Shop"]} за {item["Price"]}$.\n";
                    }
                    await ctx.RespondAsync(message);
                }
                else await ctx.RespondAsync($"{ctx.User.Mention}, время ожидания ответа истекло.");
            }
            else if (uniqueWeapons.Count == 1)
            {
                string message = default;
                foreach (var item in weaponList)
                {
                    message += $"{item["Weapon"]} продаётся в {item["Shop"]} за {item["Price"]}$.\n";
                }
                await ctx.RespondAsync(message);
            }
            else await ctx.RespondAsync("Совпадений не найдено, возможно ошибка в написании?");
        }
    }
}