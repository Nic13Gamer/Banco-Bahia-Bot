using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace BancoBahiaBot
{
    public static class Bot
    {
        public static readonly bool DEBUG = true; // change in release
        public static readonly LogSeverity API_LOG_LEVEL = LogSeverity.Info; // change in release; default is LogSeverity.Info

        public static readonly string DATA_PATH = "C:/Users/nicho/Desktop/Banco Bahia/Data"; //Directory.GetCurrentDirectory() + "/Data";  THIS IS A OVERRIDE FOR DEVELOPMENT
        public static readonly string WEBSITE = "http://localhost:5500"; // THIS IS A OVERRIDE FOR DEVELOPMENT
        public static readonly string WEBSITE_API = "http://localhost:3000"; // THIS IS A OVERRIDE FOR DEVELOPMENT

        public static readonly SaveManager.BotOptions BotOptions = SaveManager.LoadBotOptions();
        public static readonly string API_KEY = BotOptions.apiKey;

        public static DiscordSocketClient Client { get; private set; }
        public static InteractionService InteractionService { get; private set; }

        public static async Task Start()
        {
            Console.Title = "Banco Bahia Bot";
            Terminal.Start();

            Client = new(new() { LogLevel = API_LOG_LEVEL });
            InteractionService = new(Client);

            Client.Ready += async () =>
            {
                if (DEBUG)
                    await InteractionService.RegisterCommandsToGuildAsync(805241408544964669, true); // ursinhus luminosus SERVER
                else
                    await InteractionService.RegisterCommandsGloballyAsync(true);

                // start handlers that need the bot to be ready

                SaveManager.LoadAll();

                //ReactionHandler.Start();

                // ---

                Terminal.WriteLine("Bot started successfully!", Terminal.MessageType.INFO);

                await Client.SetGameAsync(/*"Sou um banco que tem seu próprio dinheiro virtual e muito mais!"*/ "com nova versao!");
            };

            Client.Log += (log) =>
            {
                Terminal.WriteLine(log, Terminal.MessageType.API);
                return null;
            };

            await Client.LoginAsync(TokenType.Bot, BotOptions.token);
            await Client.StartAsync();

            // start all handlers

            //ItemHandler.Start();
            //PropertyHandler.Start();
            //StockHandler.Start();

            await CommandHandler.Start();

            // ---

            await Task.Delay(-1);
        }
    }
}
