using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;

namespace LuckyBot
{
    class Program
    {
        public static DiscordClient discord;
        public static CommandsNextModule commands;
        public static string connectionString;
        public static string discordToken;
        public static readonly string filePath = $@"\Connection Info.txt";
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
                    connectionString = sr.ReadLine();
                    discordToken = sr.ReadLine();
                }
            }
            else Console.WriteLine("Connection Info.txt does not exist");
        }
    }
}