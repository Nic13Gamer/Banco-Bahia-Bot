using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace BancoBahiaBot
{
    public static class CommandHandler
    {
        static readonly DiscordSocketClient client = Bot.Client;
        static readonly InteractionService interactionService = Bot.InteractionService;

        public static async Task Start()
        {
            await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            client.InteractionCreated += HandleInteraction;

            interactionService.SlashCommandExecuted += SlashCommandExecuted;
        }

        static Task SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }

            return Task.CompletedTask;
        }

        static async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                var ctx = new SocketInteractionContext(client, arg);
                await interactionService.ExecuteCommandAsync(ctx, null);
            }
            catch (Exception ex)
            {
                Terminal.WriteLine(ex, Terminal.MessageType.WARN);

                if (arg.Type == InteractionType.ApplicationCommand)
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}
