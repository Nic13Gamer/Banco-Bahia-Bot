using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
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

        public static IServiceProvider Services { get; private set; }

        public static DiscordSocketClient Client { get; private set; }
        public static InteractionService InteractionService { get; private set; }

        public async Task Start()
        {
            Console.Title = "Banco Bahia Bot";

            Services = ConfigureServices();

            Client = Services.GetRequiredService<DiscordSocketClient>();
            InteractionService = Services.GetRequiredService<InteractionService>();

            Client.Ready += async () =>
            {
                if (Debug)
                    await InteractionService.RegisterCommandsToGuildAsync(805241408544964669, true); // ursinhus luminosus SERVER
                else
                    await InteractionService.RegisterCommandsGloballyAsync(true);


                Terminal.Start();

                Terminal.WriteLine("Bot started successfully!", Terminal.MessageType.INFO);

                await Client.SetGameAsync(/*"Sou um banco que tem seu próprio dinheiro virtual e muito mais!"*/ "nova versao!");
            };

            await Client.LoginAsync(TokenType.Bot, BotOptions.token);
            await Client.StartAsync();

            // start all handlers

            await CommandHandler.Start();

            // ---

            await Task.Delay(-1);
        }

        static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .BuildServiceProvider();
        }
    }
}
