using Discord.Interactions;
using System.Threading.Tasks;

namespace BancoBahiaBot.Modules
{
    public class DebugCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("debug", "debug test slash command")]
        public async Task DebugCommand()
        {
            await RespondAsync("it works!", ephemeral: true);
        }
    }
}
