using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoBahiaBot.Modules
{
    public class DebugCommands : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("DebugCmd", "debug test slash command")]
        public async Task DebugCommand()
        {
            await RespondAsync("it works!");
        }
    }
}
