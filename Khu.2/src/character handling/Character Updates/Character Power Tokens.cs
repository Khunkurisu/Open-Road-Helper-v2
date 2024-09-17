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
            return guild.GetPlayerTokenCount(User) >= 1;
        }

        public async Task BuyGold(SocketMessageComponent button)
        {
            Guild guild = Manager.GetGuild(Guild);
            uint tokens = guild.GetPlayerTokenCount(User);

            int currentLevel = _lastTokenTrade + 1;
            List<string> metadata = new() { $"{currentLevel}" };
            Dictionary<long, int> payPerLevel = new();
            int cost = 0;
            for (long startLevel = _lastTokenTrade + 1; startLevel <= _level; startLevel++)
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
                .WithTitle("PT to Gold")
                .AddField("Current", $"Levels: 1; Tokens: 2")
                .AddField("Payout", $"+{payout} gp");
            await button.UpdateAsync(x =>
            {
                x.Content = "How many levels worth of tokens would you like to spend?";
                x.Embed = embed.Build();
                x.Components = QuantityButtons(
                        "Gold",
                        metadata,
                        new()
                        {
                            currentLevel - 5 <= _lastTokenTrade,
                            currentLevel - 1 <= _lastTokenTrade,
                            currentLevel + 1 > _level,
                            currentLevel + 5 > _level
                        }
                    )
                    .WithButton(ConfirmButton("confirmBuyGold", metadata), row: 1)
                    .WithButton(CancelButton("cancelBuyGold", metadata), row: 1)
                    .Build();
            });
        }

        public async Task ModifyGoldFromPT(SocketMessageComponent button)
        {
            Guild guild = Manager.GetGuild(Guild);
            uint tokens = guild.GetPlayerTokenCount(User);

            FormValue formValues = guild.GetFormValues(button.Data.CustomId);

            ulong playerId = formValues.User;
            IUser user = button.User;
            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await InteractionErrors.MissingPermissions(button, user);
                return;
            }

            int levelChange = int.Parse(formValues.Modifier);

            List<string> metadata = formValues.Metadata;
            int currentLevel = int.Parse(metadata[0]) + levelChange;
            metadata.RemoveAt(0);
            Dictionary<int, int> payPerLevel = new();
            foreach (string data in metadata)
            {
                string[] values = data.Split(":");
                payPerLevel.Add(int.Parse(values[0]), int.Parse(values[1]));
                await Task.Yield();
            }
            metadata = new() { $"{currentLevel}" };
            foreach (int lvl in payPerLevel.Keys)
            {
                metadata.Add($"{lvl}:{payPerLevel[lvl]}");
            }

            int payout = payPerLevel[currentLevel];
            for (int level = _lastTokenTrade + 1; level <= currentLevel; level++)
            {
                payout += payPerLevel[level];
            }
            int levelCount = currentLevel - _lastTokenTrade;
            var embed = new EmbedBuilder()
                .WithTitle("PT to Gold")
                .AddField("Current", $"Levels: {levelCount}; Tokens: {levelCount * 2} PT")
                .AddField("Payout", $"+{payout} gp");
            await button.UpdateAsync(x =>
            {
                x.Content = "How many levels worth of tokens would you like to spend?";
                x.Embed = embed.Build();
                x.Components = QuantityButtons(
                        "Gold",
                        metadata,
                        new()
                        {
                            currentLevel - 5 <= _lastTokenTrade,
                            currentLevel - 1 <= _lastTokenTrade,
                            currentLevel + 1 > _level,
                            currentLevel + 5 > _level
                        }
                    )
                    .WithButton(ConfirmButton("confirmBuyGold", metadata), row: 1)
                    .WithButton(CancelButton("cancelBuyGold", metadata), row: 1)
                    .Build();
            });
        }

        public async Task ConfirmBuyGold(SocketMessageComponent button)
        {
            Guild guild = Manager.GetGuild(Guild);
            uint tokens = guild.GetPlayerTokenCount(User);

            FormValue formValues = guild.GetFormValues(button.Data.CustomId);

            IThreadChannel? transactionChannel = (IThreadChannel?)
                Manager.GetTextChannel(guild.Id, TransactionThread);
            if (transactionChannel == null)
            {
                await InteractionErrors.CharacterThreadNotFound(button, Name);
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

            int payout = payPerLevel[currentLevel];
            List<int> levelsPaidList = new();
            for (int level = _lastTokenTrade + 1; level <= currentLevel; level++)
            {
                payout += payPerLevel[level];
                levelsPaidList.Add(level);
            }
            int levelCount = currentLevel - _lastTokenTrade;
            string levelsPaid = string.Join(", ", levelsPaidList);
            _lastTokenTrade = currentLevel;

            guild.DecreasePlayerTokenCount(User, (uint)(levelCount * 2));
            guild.QueueSave(SaveType.Tokens);
            Gold += payout;
            guild.QueueSave(SaveType.Characters);

            var embed = new EmbedBuilder()
                .WithTitle("PT to Gold")
                .AddField("Level Range", $"Levels {levelsPaid}")
                .AddField("Cost", $"Tokens: {levelCount * 2} PT")
                .AddField("Payout", $"+{payout} gp");
            await button.UpdateAsync(x =>
            {
                x.Content =
                    $"Gold purchased using {levelCount * 2} PT. ({Gold} GP | {guild.GetPlayerTokenCount(User)} PT)";
                x.Embed = embed.Build();
                x.Components = null;
            });
            await transactionChannel.SendMessageAsync(
                $"Gold purchased using {levelCount * 2} PT. ({Gold} GP | {guild.GetPlayerTokenCount(User)} PT)",
                embed: embed.Build()
            );

            await guild.RefreshCharacterPosts(User);
        }

        public async Task BuyDowntime(SocketMessageComponent button)
        {
            Guild guild = Manager.GetGuild(Guild);
            uint tokens = guild.GetPlayerTokenCount(User);
            int purchaseCount = 1;
            List<string> metadata = new() { $"{purchaseCount}" };

            int payout = purchaseCount * 5;
            var embed = new EmbedBuilder()
                .WithTitle("PT to Downtime")
                .AddField("Current", $"{purchaseCount} PT")
                .AddField("Payout", $"+{payout} DT");
            await button.RespondAsync(
                "How many levels worth of tokens would you like to spend?",
                embed: embed.Build(),
                components: QuantityButtons(
                        "Downtime",
                        metadata,
                        new()
                        {
                            purchaseCount - 5 < 1,
                            purchaseCount - 1 < 1,
                            purchaseCount + 1 > tokens,
                            purchaseCount + 5 > tokens
                        }
                    )
                    .WithButton(ConfirmButton("confirmBuyDowntime", metadata), row: 1)
                    .WithButton(CancelButton("cancelBuyDowntime", metadata), row: 1)
                    .Build(),
                ephemeral: true
            );
        }

        public async Task ModifyDowntimeFromPT(SocketMessageComponent button)
        {
            Guild guild = Manager.GetGuild(Guild);
            uint tokens = guild.GetPlayerTokenCount(User);

            FormValue formValues = guild.GetFormValues(button.Data.CustomId);

            ulong playerId = formValues.User;
            IUser user = button.User;
            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await InteractionErrors.MissingPermissions(button, user);
                return;
            }

            int levelChange = int.Parse(formValues.Modifier);

            List<string> metadata = formValues.Metadata;
            int purchaseCount = int.Parse(metadata[0]) + levelChange;
            metadata[0] = $"{purchaseCount}";

            int payout = purchaseCount * 5;
            var embed = new EmbedBuilder()
                .WithTitle("PT to Downtime")
                .AddField("Current", $"{purchaseCount} PT")
                .AddField("Payout", $"+{payout} DT");
            await button.UpdateAsync(x =>
            {
                x.Content = "How many levels worth of tokens would you like to spend?";
                x.Embed = embed.Build();
                x.Components = QuantityButtons(
                        "Downtime",
                        metadata,
                        new()
                        {
                            purchaseCount - 5 < 1,
                            purchaseCount - 1 < 1,
                            purchaseCount + 1 > tokens,
                            purchaseCount + 5 > tokens
                        }
                    )
                    .WithButton(ConfirmButton("confirmBuyDowntime", metadata), row: 1)
                    .WithButton(CancelButton("cancelBuyDowntime", metadata), row: 1)
                    .Build();
            });
        }

        public async Task ConfirmBuyDowntime(SocketMessageComponent button)
        {
            Guild guild = Manager.GetGuild(Guild);
            uint tokens = guild.GetPlayerTokenCount(User);

            FormValue formValues = guild.GetFormValues(button.Data.CustomId);

            IThreadChannel? transactionChannel = (IThreadChannel?)
                Manager.GetTextChannel(guild.Id, TransactionThread);
            if (transactionChannel == null)
            {
                await InteractionErrors.CharacterThreadNotFound(button, Name);
                return;
            }

            List<string> metadata = formValues.Metadata;
            int purchaseCount = int.Parse(metadata[0]);

            int payout = purchaseCount * 5;

            guild.DecreasePlayerTokenCount(User, (uint)purchaseCount);
            guild.QueueSave(SaveType.Tokens);
            Downtime += (uint)payout;
            guild.QueueSave(SaveType.Characters);

            var embed = new EmbedBuilder()
                .WithTitle("PT to Downtime")
                .AddField("Cost", $"Tokens: {purchaseCount} PT")
                .AddField("Payout", $"+{payout} DT");
            await button.UpdateAsync(x =>
            {
                x.Content =
                    $"Downtime purchased using {purchaseCount} PT. ({Downtime} DT | {guild.GetPlayerTokenCount(User)} PT)";
                x.Embed = embed.Build();
                x.Components = null;
            });
            await transactionChannel.SendMessageAsync(
                $"Downtime purchased using {purchaseCount} PT. ({Downtime} DT | {guild.GetPlayerTokenCount(User)} PT)",
                embed: embed.Build()
            );

            await guild.RefreshCharacterPosts(User);
        }
    }
}
