using Discord;
using Discord.Interactions;
using System;
using System.Threading.Tasks;

namespace BancoBahiaBot.Modules
{
    public class MoneyCommands : InteractionModuleBase<SocketInteractionContext>
    {
        readonly Random random = new();

        [SlashCommand("atm", "Mostra o seu dinheiro ou o de outro usuário")]
        public async Task AtmCommand([Summary("user")]IGuildUser mention = null)
        {
            var discordUser = (IUser)mention ?? Context.User;
            if (discordUser.IsBot) return;

            User user = UserHandler.GetUser(discordUser.Id);
            string reply = $"{discordUser.Mention} tem ${user.money}!";

            await RespondAsync(reply, ephemeral: true);
        }

        [SlashCommand("pay", "Transfira a quantidade escolhida de dinheiro para outro usuário")]
        public async Task PayCommand([Summary("user")]IGuildUser mention, int quantity)
        {
            if (mention.IsBot || mention == Context.User) return;

            if (quantity <= 0)
            {
                await RespondAsync("A quantia mínima é $1!", ephemeral: true);
                return;
            }

            User user = UserHandler.GetUser(Context.User.Id);
            User mentionUser = UserHandler.GetUser(mention.Id);

            if (user.money < quantity)
            {
                await RespondAsync("Você não tem dinheiro suficiente!", ephemeral: true);
                return;
            }

            user.money -= quantity;
            mentionUser.money += quantity;

            Terminal.WriteLine($"{Context.User} ({Context.User.Id}) transfered {quantity} to {mention} ({mention.Id})", Terminal.MessageType.INFO);

            await RespondAsync($"{Context.User.Mention} transferiu ${quantity} para {mention.Mention}!");
        }

        [SlashCommand("steal", "Tente roubar dinheiro de outro usuário, mas tome cuidado com a polícia 🚓")]
        public async Task StealCommand([Summary("user")]IGuildUser mention)
        {
            if (mention.IsBot || mention == Context.User) return;

            if (UserHandler.GetUser(Context.User.Id).money < 2000)
            {
                await RespondAsync("Você deve ter no mínimo 2000 de dinheiro para poder roubar alguém!", ephemeral: true);
                return;
            }

            int money = random.Next(120, 1000);
            bool success = Convert.ToBoolean(random.Next(0, 2));

            await RespondAsync($"{Context.User.Mention} tentou roubar {money} de {mention.Mention}!");
            
            await Task.Delay(2000);

            if (UserHandler.GetUser(mention.Id).money < money)
            {
                await ReplyAsync($"{mention.Mention} não tinha {money} de dinheiro, então, o roubo foi cancelado.");
                await DeleteOriginalResponseAsync();
                return;
            }

            if (success)
            {
                UserHandler.GetUser(Context.User.Id).money += money;
                UserHandler.GetUser(mention.Id).money -= money;

                await ReplyAsync($"{Context.User.Mention} conseguiu roubar {money} de {mention.Mention}! {Context.User.Mention} ganhou {money}.");
                return;
            }
            else
            {
                UserHandler.GetUser(Context.User.Id).money -= money * 2;
                UserHandler.GetUser(mention.Id).money += money * 2;

                await ReplyAsync($"{Context.User.Mention} fracassou o roubo e teve que pagar uma multa do dobro do dinheiro roubado! {mention.Mention} ganhou {money * 2}.");
            }

            await DeleteOriginalResponseAsync();
        }
    }
}
