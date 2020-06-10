using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
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
                        message += $"{item["Weapon"]} продаётся в {item["Shop"]} за {item["Price"].ToDouble().ToString("N0", CultureInfo.GetCultureInfo("ru-RU"))}₽.\n";
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
                    message += $"{item["Weapon"]} продаётся в {item["Shop"]} за {item["Price"].ToDouble().ToString("N0", CultureInfo.GetCultureInfo("ru-RU"))}₽.\n";
                }

                await ctx.RespondAsync(message);
            }
            else await ctx.RespondAsync("Совпадений не найдено, возможно ошибка в написании?");
        }

        [Command("clear"), Aliases("cl"), Description("Удаление последних сообщений"),
         RequirePermissions(Permissions.ManageMessages)]
        public async Task ClearMessages(CommandContext ctx, [Description("Количество сообщений к удалению")]
            int amount)
        {
            if (amount < 1)
            {
                await ctx.RespondAsync("Некорректное число сообщений, попробуйте снова.");
            }
            else if (amount > 100)
            {
                await ctx.RespondAsync("Удаление не выполнено. Нельзя удалять больше 100 сообщений за раз.");
            }
            else
            {
                List<DiscordMessage> trashMessages =
                    new List<DiscordMessage>(ctx.Channel.GetMessagesAsync(amount, ctx.Channel.LastMessageId).Result);
                trashMessages.Add(ctx.Message);
                await ctx.Channel.DeleteMessagesAsync(trashMessages);
                var resultMessage = ctx.RespondAsync($"Удалено {trashMessages.Count - 1} сообщений.").Result;
                Thread.Sleep(4000);
                await resultMessage.DeleteAsync();
            }
        }

        [Command("trash"), Description("Удаление всех сообщений вплоть до помеченного эмотом :trash_can:"),
         RequirePermissions(Permissions.ManageMessages)]
        public async Task ClearTrash(CommandContext ctx)
        {
            DiscordEmoji emoji = DiscordEmoji.FromName(ctx.Client, ":trash_can:");
            var messages = ctx.Channel.GetMessagesAsync(100, ctx.Channel.LastMessageId).Result;
            DiscordMessage targetMessage = null;
            foreach (var item in messages)
            {
                foreach (var item2 in item.Reactions)
                {
                    if (item2.Emoji.Name == emoji.Name)
                    {
                        targetMessage = item;
                        break;
                    }
                }

                if (targetMessage != null) break;
            }

            if (targetMessage != null)
            {
                List<DiscordMessage> trashMessages =
                    new List<DiscordMessage>(ctx.Channel.GetMessagesAsync(after: targetMessage.Id).Result);
                trashMessages.Add(targetMessage);
                await ctx.Channel.DeleteMessagesAsync(trashMessages);
                var resultMessage = ctx.RespondAsync($"Удалено {trashMessages.Count - 1} сообщений.").Result;
                Thread.Sleep(4000);
                await ctx.Channel.DeleteMessageAsync(resultMessage);
            }
            else await ctx.RespondAsync("Мусор не найден.");
        }
    }
}