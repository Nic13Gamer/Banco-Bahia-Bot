using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace BancoBahiaBot
{
    public class Bot
    {
        public static bool Debug { get; private set; } = true; // change this in release

        public static readonly string DATA_PATH = "C:/Users/nicho/Desktop/Banco Bahia/Data"; //Directory.GetCurrentDirectory() + "/Data";  THIS IS A OVERRIDE FOR DEVELOPMENT
        public static readonly string WEBSITE = "http://localhost:5500"; // THIS IS A OVERRIDE FOR DEVELOPMENT
        public static readonly string WEBSITE_API = "http://localhost:3000"; // THIS IS A OVERRIDE FOR DEVELOPMENT

        public static readonly SaveManager.BotOptions BotOptions = SaveManager.LoadBotOptions();
        public static readonly string API_KEY = BotOptions.apiKey;

        public static DiscordSocketClient Client { get; private set; }
        public static InteractionService InteractionService { get; private set; }

        public async Task Start()
        {
            Console.Title = "Banco Bahia Bot";

            Client = new(new() { GatewayIntents = GatewayIntents.All });
            InteractionService = new(Client);

            Client.Ready += async () =>
            {
                if (Debug)
                    await InteractionService.RegisterCommandsToGuildAsync(805241408544964669, true); // ursinhus luminosus SERVER
                else
                    await InteractionService.RegisterCommandsGloballyAsync(true);


                Terminal.Start();

                Terminal.WriteLine("Bot started successfully!", Terminal.MessageType.INFO);

                await Client.SetGameAsync(/*"Sou um banco que tem seu próprio dinheiro virtual e muito mais!"*/ "COM VERSAO NOVA!");
            };

            await Client.LoginAsync(TokenType.Bot, BotOptions.token);
            await Client.StartAsync();

            // start all handlers

            await CommandHandler.Start();

            // ---

            await Task.Delay(-1);
        }
    }
}
