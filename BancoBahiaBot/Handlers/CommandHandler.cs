using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
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

        static async Task SlashCommandExecuted(SlashCommandInfo command, IInteractionContext context, IResult result)
        {
            if (!result.IsSuccess && result.Error != InteractionCommandError.UnknownCommand)
            {
                string reply = $"Alguma coisa deu errado! Motivo: " + result.ErrorReason;

                Terminal.WriteLine($"Bot use error [{result.ErrorReason}] by {context.User} ({context.User.Id})", Terminal.MessageType.WARN);
                
                await context.Interaction.RespondAsync(reply, ephemeral: true);
                return;
            }
            
            SaveManager.SaveAll();
        }

        static Task HandleInteraction(SocketInteraction arg)
        {
            var context = new SocketInteractionContext(client, arg);

            if(context.User.IsBot || context.Guild == null) return null;
            UserHandler.CreateUser(context.User.Id);

            // execute the command
            interactionService.ExecuteCommandAsync(context, null);

            return null;
        }
    }
}
