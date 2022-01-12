using BancoBahiaBot.Utils;

using Discord;
using Discord.Interactions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BancoBahiaBot.Modules
{
    public class StockCommands : InteractionModuleBase<SocketInteractionContext>
    {
        static readonly bool isStocksOpen = StockHandler.isStocksOpen;

        [Group("broker", "Mexa em seu portfólio")]
        public class BrokerCommandsGroup : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("see", "Veja os tickers da bolsa de valores")]
            public async Task SeeCommand()
            {
                EmbedBuilder embed = new EmbedBuilder
                {
                    Title = $"**TICKERS DISPONÍVEIS**",
                    Description = "A bolsa atualiza a cada 5 minutos.",
                    Color = Color.Orange
                }.WithCurrentTimestamp().WithFooter(footer => { footer.Text = "Para ver mais sobre um ticker, use ?broker info <ticker>"; });

                foreach (Stock stock in StockHandler.GetStocks())
                {
                    string wentUpEmoji = stock.wentUp ? ":arrow_up:" : ":arrow_down:";

                    embed.AddField($"{stock.name} `({stock.shortName})` {wentUpEmoji}",
                        $"Preço: **`${stock.price}`**", true);
                }

                await RespondAsync(embed: embed.Build(), ephemeral: true);
            }

            [SlashCommand("portfolio", "Veja seu portfólio")]
            public async Task PortfolioCommand()
            {
                User user = UserHandler.GetUser(Context.User.Id);

                EmbedBuilder embed = new EmbedBuilder
                {
                    Title = $"**PORTIFÓLIO**",
                    Description = "A bolsa atualiza a cada 5 minutos.",
                    Color = Color.Orange
                }.WithAuthor(Context.User).WithCurrentTimestamp().WithFooter(footer => { footer.Text = "Para ver mais sobre um ticker, use ?broker info <ticker>"; });

                foreach (UserStock userStock in user.stocks)
                {
                    Stock stock = userStock.stock;

                    string wentUpEmoji = stock.wentUp ? ":arrow_up:" : ":arrow_down:";
                    string stockSuccessEmoji = stock.price * userStock.quantity > userStock.highBuyPrice * userStock.quantity ? ":green_circle:" : ":red_circle:";

                    string totalString = $"Total: **`${stock.price * userStock.quantity}`**" +
                        $" | **`{(stock.price * userStock.quantity) - (userStock.highBuyPrice * userStock.quantity)}`** de ganho";

                    embed.AddField($"{stock.name} `({stock.shortName})` {wentUpEmoji} | {stockSuccessEmoji}",
                        totalString +
                        $"\nMaior preço de compra: **`${userStock.highBuyPrice}`** | **`${userStock.highBuyPrice * userStock.quantity}`**" +
                        $"\nQuantidade: **`{userStock.quantity}`**", true);
                }

                await RespondAsync(embed: embed.Build(), ephemeral: true);
            }

            [SlashCommand("buy", "Compre ações para seu portfólio")]
            public async Task BuyCommand([Summary("ticker")]string stockString, int quantity = 1)
            {
                if (!isStocksOpen)
                {
                    await RespondAsync("A bolsa de valores não está aberta agora!", ephemeral: true);
                    return;
                }

                User user = UserHandler.GetUser(Context.User.Id);

                if (quantity < 1)
                {
                    await RespondAsync("A quantidade deve ser maior que 0!", ephemeral: true);
                    return;
                }

                Stock stock = StockHandler.GetStock(stockString);
                if (stock == null)
                {
                    await RespondAsync("Esse ticker não existe!", ephemeral: true);
                    return;
                }

                int totalPrice = stock.price * quantity;
                if (user.money < totalPrice)
                {
                    await RespondAsync("Você não tem dinheiro suficiente!", ephemeral: true);
                    return;
                }

                user.money -= totalPrice;
                StockHandler.AddStockToUser(user, stock, quantity);

                string reply = $"Você comprou {quantity} ações de `{stock.name} ({stock.shortName})` por `${totalPrice}`!";
                if (quantity == 1)
                    reply = $"Você comprou {quantity} ação de `{stock.name} ({stock.shortName})` por `${totalPrice}`!";

                await RespondAsync(reply, ephemeral: true);
                Terminal.WriteLine($"{Context.User} ({Context.User.Id}) bought {quantity} {stock.id} for ${totalPrice}!", Terminal.MessageType.INFO);
            }

            [SlashCommand("sell", "Venda ações de seu portfólio")]
            public async Task SellCommand([Summary("ticker")] string stockString, int quantity = 1)
            {
                if (!isStocksOpen)
                {
                    await RespondAsync("A bolsa de valores não está aberta agora!", ephemeral: true);
                    return;
                }

                User user = UserHandler.GetUser(Context.User.Id);

                if (quantity < 1)
                {
                    await RespondAsync("A quantidade deve ser maior que 0!", ephemeral: true);
                    return;
                }

                Stock stock = StockHandler.GetStock(stockString);
                if (stock == null)
                {
                    await RespondAsync("Esse ticker não existe!", ephemeral: true);
                    return;
                }
                UserStock userStock = StockHandler.GetUserStock(stock, user);
                if (userStock == null)
                {
                    await RespondAsync("Você não possui essa ação!", ephemeral: true);
                    return;
                }
                if (userStock.quantity < quantity)
                {
                    await RespondAsync("Você não possui ações suficientes!", ephemeral: true);
                    return;
                }

                int totalPrice = stock.price * quantity;

                user.money += totalPrice;
                StockHandler.RemoveStockFromUser(user, stock, quantity);

                string reply = $"Você vendeu {quantity} ações de `{stock.name} ({stock.shortName})` por `${totalPrice}`!";
                if (quantity == 1)
                    reply = $"Você vendeu {quantity} ação de `{stock.name} ({stock.shortName})` por `${totalPrice}`!";

                await RespondAsync(reply, ephemeral: true);
                Terminal.WriteLine($"{Context.User} ({Context.User.Id}) sold {quantity} {stock.id} for ${totalPrice}!", Terminal.MessageType.INFO);
            }

            //[SlashCommand("info", "TODO")]
        }

        /*static string GetStockLastPricesString(Stock stock)
        {
            string data = string.Empty;
            foreach (int price in stock.lastPrices)
            {
                if (data == string.Empty)
                    data += price;
                else
                    data += "," + price;
            }

            return data;
        }*/ // use in stock info
    }
}
