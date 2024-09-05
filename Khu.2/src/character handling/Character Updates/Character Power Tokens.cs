using Discord;
using Discord.WebSocket;
using OpenRoadHelper.Guilds;

namespace OpenRoadHelper.Characters
{
    public partial class Character
    {
        public bool CanTradePT2GP()
        {
            Guild guild = Manager.GetGuild(Guild);
            return LastTokenTrade < Level && guild.GetPlayerTokenCount(User) >= 2;
        }

        public bool CanTradePT2DT()
        {
            Guild guild = Manager.GetGuild(Guild);
            return guild.GetPlayerTokenCount(User) >= 2;
        }

        public async Task BuyGold(SocketMessageComponent button)
        {
            Guild guild = Manager.GetGuild(Guild);
            uint tokens = guild.GetPlayerTokenCount(User);

            int currentLevel = _lastTokenTrade + 1;
            List<string> metadata = new() { $"{currentLevel}" };
            Dictionary<long, int> payPerLevel = new();
            int cost = 0;
            for (long startLevel = _lastTokenTrade + 1; startLevel < _level; startLevel++)
            {
                cost += 2;
                if (cost > tokens)
                    break;

                metadata.Add($"{startLevel}:{PF2E.CharacterWealth[startLevel, 1]}");
                payPerLevel.Add(startLevel, PF2E.CharacterWealth[startLevel, 1]);
                await Task.Yield();
            }

            if (!metadata.Any())
            {
                await button.RespondAsync(
                    $"Unable to afford any gold. ({tokens} PT)",
                    ephemeral: true
                );
                return;
            }
            int payout = payPerLevel[currentLevel];
            var embed = new EmbedBuilder()
                .WithTitle("PT 2 Gold")
                .AddField("Current", $"Levels: 1; Tokens: 2")
                .AddField("Payout", $"{payout}");
            await button.RespondAsync(
                "How many levels worth of tokens would you like to spend?",
                embed: embed.Build(),
                components: QuantityButtons(
                        "Gold",
                        metadata,
                        new()
                        {
                            currentLevel - 5 <= _lastTokenTrade,
                            currentLevel - 1 <= _lastTokenTrade,
                            currentLevel + 1 >= _level,
                            currentLevel + 5 >= _level
                        }
                    )
                    .Build(),
                ephemeral: true
            );
        }

        public async Task IncrementGold(SocketMessageComponent button)
        {
            Guild guild = Manager.GetGuild(Guild);
            uint tokens = guild.GetPlayerTokenCount(User);

            FormValue formValues = guild.GetFormValues(button.Data.CustomId);

            ulong playerId = formValues.User;
            IUser user = button.User;
            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await button.RespondAsync("You lack permission to do that!", ephemeral: true);
                return;
            }

            List<string> metadata = formValues.Metadata;
            int currentLevel = int.Parse(metadata[0]);
            metadata.RemoveAt(0);
            Dictionary<int, int> payPerLevel = new();
            foreach (string data in metadata)
            {
                string[] values = data.Split(":");
                payPerLevel.Add(int.Parse(values[0]), int.Parse(values[1]));
                await Task.Yield();
            }
        }

        public async Task DecrementGold(SocketMessageComponent button)
        {
            await Task.Yield();
        }

        public async Task ConfirmBuyGold(SocketMessageComponent button)
        {
            Guild guild = Manager.GetGuild(Guild);
            uint tokens = guild.GetPlayerTokenCount(User);

            FormValue formValues = guild.GetFormValues(button.Data.CustomId);

            IThreadChannel charThread = (IThreadChannel)button.Channel;
            IThreadChannel? transactionChannel = (IThreadChannel?)
                Manager.GetTextChannel(guild.Id, TransactionThread);
            if (transactionChannel == null || charThread == null)
            {
                await button.RespondAsync(
                    $"{Name} forum threads could not be found.",
                    ephemeral: true
                );
                return;
            }
        }

        public async Task BuyDowntime(SocketMessageComponent button)
        {
            await Task.Yield();
            Guild guild = Manager.GetGuild(Guild);
            uint tokens = guild.GetPlayerTokenCount(User);
        }

        public async Task IncrementDowntime(SocketMessageComponent button)
        {
            await Task.Yield();
        }

        public async Task DecrementDowntime(SocketMessageComponent button)
        {
            await Task.Yield();
        }

        public async Task ConfirmBuyDowntime(SocketMessageComponent button)
        {
            Guild guild = Manager.GetGuild(Guild);
            uint tokens = guild.GetPlayerTokenCount(User);

            FormValue formValues = guild.GetFormValues(button.Data.CustomId);

            IThreadChannel charThread = (IThreadChannel)button.Channel;
            IThreadChannel? transactionChannel = (IThreadChannel?)
                Manager.GetTextChannel(guild.Id, TransactionThread);
            if (transactionChannel == null || charThread == null)
            {
                await button.RespondAsync(
                    $"{Name} forum threads could not be found.",
                    ephemeral: true
                );
                return;
            }
        }
    }
}
