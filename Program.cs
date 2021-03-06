﻿using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;

namespace LuckyBot
{
    class Program
    {
        private static DiscordClient discord;
        private static InteractivityModule interactivity;
        private static CommandsNextModule commands;
        private static readonly string filePath = "Connection Info.txt";
        private static string discordToken;
        public static string connectionString;

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            InitConnectionStrings();
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = discordToken,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });
            interactivity = discord.UseInteractivity(new InteractivityConfiguration());
            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });
            commands.RegisterCommands<BotCommands>();
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        static void InitConnectionStrings()
        {
            if (File.Exists(filePath))
            {
                using (StreamReader sr = new StreamReader(filePath, Encoding.Default))
                {
                    string text = sr.ReadToEnd();
                    connectionString = Regex.Match(text, @"MongoDB: (.*)\r\n").Groups[1].ToString().Trim();
                    discordToken = Regex.Match(text, @"Discord: (.*)").Groups[1].ToString().Trim();
                }
            }
            else Console.WriteLine("Connection Info.txt does not exist");
        }
    }
}