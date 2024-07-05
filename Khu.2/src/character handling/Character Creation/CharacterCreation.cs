using Bot.Characters;
using Bot.Guilds;
using Discord;
using Discord.WebSocket;

namespace Bot
{
    public partial class Manager
    {
        public static void StoreTempCharacter(
            IImportable characterImport,
            ulong guildId,
            IUser player,
            float height,
            float weight
        )
        {
            Guild guild = GetGuild(guildId);
            if (guild == null)
            {
                return;
            }
            guild.StoreTempCharacter(player.Id, characterImport, height, weight);
        }

        public static void StoreTempCharacter(
            IImportable characterImport,
            ulong guildId,
            IUser player
        )
        {
            Guild guild = GetGuild(guildId);
            if (guild == null)
            {
                return;
            }
            guild.StoreTempCharacter(player.Id, characterImport);
        }

        private static async Task CharacterCreate(
            SocketModal modal,
            List<SocketMessageComponentData> components,
            bool isForced = false
        )
        {
            ulong? guildIdOrNull = modal.GuildId;
            if (guildIdOrNull == null)
            {
                await modal.RespondAsync("This command can only be used in a guild.");
                return;
            }
            ulong guildId = (ulong)guildIdOrNull;
            Guild guild = GetGuild(guildId);
            List<dynamic> formData = guild.GetFormValues(modal.Data.CustomId);
            string playerIdString = formData[2];
            ulong playerId = ulong.Parse(playerIdString);
            IUser user = modal.User;
            IUser? player = _client?.GetUser(playerId);
            if (player == null)
            {
                await modal.RespondAsync("Unable to locate player.", ephemeral: true);
                return;
            }

            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await modal.RespondAsync(
                    "You lack permissions to perform this action.",
                    ephemeral: true
                );
                return;
            }
            string charName = components.First(x => x.CustomId == "character_name").Value;
            string charDescription = components
                .First(x => x.CustomId == "character_description")
                .Value;
            string charReputation = components
                .First(x => x.CustomId == "character_reputation")
                .Value;

            Dictionary<string, dynamic>? charData;
            IImportable? importedCharacter = guild.GetCharacterJson(playerId);
            if (importedCharacter != null)
            {
                charData = importedCharacter.GetCharacterData();
                if (charData != null)
                {
                    int tokenCost = guild.GetNewCharacterCost(playerId);
                    uint tokenCount = guild.GetPlayerTokenCount(playerId);
                    if (!isForced && tokenCount < tokenCost)
                    {
                        await modal.RespondAsync(
                            "You lack sufficient tokens to create a new character. "
                                + $"({tokenCost} PT required - {tokenCount} PT remaining.)"
                        );
                        return;
                    }
                    charData["name"] = charName;
                    charData["description"] = charDescription;
                    charData["reputation"] = charReputation;
                    Character character = new(player, guild, charData);
                    guild.AddCharacter(user.Id, character);

                    await modal.RespondAsync(
                        "Ensure everything looks correct before continuing!",
                        embed: character.GenerateEmbed(user).Build(),
                        components: character.GenerateComponents().Build(),
                        ephemeral: true
                    );
                }
                else
                {
                    await modal.RespondAsync("charData was null");
                }
            }
            else
            {
                await modal.RespondAsync("importedCharacter was null");
            }
        }

        private static async Task CharacterCreateConfirm(
            SocketMessageComponent button,
            bool isForced = false
        )
        {
            if (button.GuildId == null)
            {
                await button.RespondAsync("This must be run in a guild.", ephemeral: true);
                return;
            }
            ulong guildId = (ulong)button.GuildId;
            Guild guild = GetGuild(guildId);

            List<dynamic> formValues = guild.GetFormValues(button.Data.CustomId);

            ulong playerId = formValues[1];
            IUser user = button.User;
            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await button.RespondAsync("You lack permission to do that!", ephemeral: true);
                return;
            }

            string charName = formValues[2];
            Character? character = guild.GetCharacter(user.Id, charName);
            if (character == null)
            {
                await button.UpdateAsync(x =>
                {
                    x.Content = $"Unable to locate {charName}.";
                    x.Embed = null;
                    x.Components = null;
                });
                return;
            }
            if (character.Status != Status.Temp)
            {
                await button.UpdateAsync(x =>
                {
                    x.Content = $"{charName} has an invalid status for that action.";
                    x.Embed = null;
                    x.Components = null;
                });
                return;
            }

            bool paid = true;
            if (!isForced)
            {
                paid = guild.DecreasePlayerTokenCount(
                    user.Id,
                    (uint)guild.GetNewCharacterCost(user.Id)
                );
            }
            if (paid)
            {
                await PostCharacter(button, guild, character, user);
            }
            else
            {
                await button.UpdateAsync(x =>
                {
                    x.Content = $"You lack sufficient PT to create {charName}.";
                    x.Embed = null;
                    x.Components = null;
                });
            }
        }

        private static async Task CharacterCreateCancel(SocketMessageComponent button)
        {
            if (button.GuildId == null)
            {
                await button.RespondAsync("This must be run in a guild.", ephemeral: true);
                return;
            }
            ulong guildId = (ulong)button.GuildId;
            Guild guild = GetGuild(guildId);

            List<dynamic> formValues = guild.GetFormValues(button.Data.CustomId);

            ulong playerId = formValues[1];
            IUser user = button.User;
            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await button.RespondAsync("You lack permission to do that!", ephemeral: true);
                return;
            }

            string charName = formValues[2];
            Character? character = guild.GetCharacter(user.Id, charName);
            if (character == null)
            {
                await button.UpdateAsync(x =>
                {
                    x.Content = $"Unable to locate {charName}.";
                    x.Embed = null;
                    x.Components = null;
                });
                return;
            }
            if (character.Status != Status.Temp)
            {
                await button.UpdateAsync(x =>
                {
                    x.Content = $"{charName} has an invalid status for that action.";
                    x.Embed = null;
                    x.Components = null;
                });
                return;
            }

            guild.ClearTempCharacter(user.Id);
            guild.RemoveCharacter(character);
            await button.UpdateAsync(x =>
            {
                x.Content = "Character creation canceled.";
                x.Embed = null;
                x.Components = null;
            });
        }

        private static async Task PostCharacter(
            SocketMessageComponent context,
            Guild guild,
            Character character,
            IUser user
        )
        {
            if (guild.CharacterBoard == null || guild.TransactionBoard == null)
            {
                await context.UpdateAsync(x =>
                {
                    x.Content = $"Server forum boards have not been configured.";
                    x.Embed = null;
                    x.Components = null;
                });
                return;
            }

            guild.ClearTempCharacter(user.Id);
            ForumTag[] tags =
            {
                guild.CharacterBoard.Tags.First(x => x.Name == "Pending")
            };
            character.Status = Status.Pending;

            IThreadChannel charThread = await guild.CharacterBoard.CreatePostAsync(
                character.Name,
                ThreadArchiveDuration.OneWeek,
                embed: character.GenerateEmbed(user).Build(),
                tags: tags,
                components: character.GenerateComponents().Build()
            );

            IThreadChannel transThread = await guild.TransactionBoard.CreatePostAsync(
                character.Name,
                ThreadArchiveDuration.OneWeek,
                text: $"{character.Name} Transactions"
            );

            await transThread.SendMessageAsync(
                $"{charThread.Mention} has been created and is pending approval."
            );

            character.CharacterThread = charThread.Id;
            character.TransactionThread = transThread.Id;
            guild.QueueSave("characters");

            await charThread.AddUserAsync((IGuildUser)user);
            await transThread.AddUserAsync((IGuildUser)user);

            await context.UpdateAsync(x =>
            {
                x.Content =
                    $"{character.Name} has been created and is pending approval. (Character: {charThread.Mention} | Transactions: {transThread.Mention})";
                x.Embed = null;
                x.Components = null;
            });
        }

        public static async Task PendingCharacterRefund(
            SocketMessageComponent button,
            bool isForced = false
        )
        {
            if (button.GuildId == null)
            {
                await button.RespondAsync("This must be run in a guild.", ephemeral: true);
                return;
            }
            ulong guildId = (ulong)button.GuildId;
            Guild guild = GetGuild(guildId);

            List<dynamic> formValues = guild.GetFormValues(button.Data.CustomId);

            ulong playerId = formValues[1];
            IUser user = button.User;
            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await button.RespondAsync("You lack permission to do that!", ephemeral: true);
                return;
            }

            string charName = formValues[2];
            Character? character = guild.GetCharacter(user.Id, charName);
            if (character == null)
            {
                await button.RespondAsync(
                    $"{charName} could not be found in database for {user.Username}.",
                    ephemeral: true
                );
                return;
            }
            if (character.Status != Status.Pending && character.Status != Status.Rejected)
            {
                await button.RespondAsync(
                    $"{charName} could not be refunded due to incorrect status.",
                    ephemeral: true
                );
                return;
            }

            IThreadChannel charThread = (IThreadChannel)button.Channel;
            IThreadChannel? transactionChannel = (IThreadChannel?)GetTextChannel(
                guild.Id,
                character.TransactionThread
            );
            if (transactionChannel == null || charThread == null)
            {
                await button.RespondAsync(
                    $"{charName} forum threads could not be found.",
                    ephemeral: true
                );
                return;
            }

            guild.RemoveCharacter(character);
            string transactionMessage = "Forced character creation canceled.";
            if (!isForced)
            {
                uint refundAmount = (uint)guild.GetNewCharacterCost(user.Id);
                guild.IncreasePlayerTokenCount(user.Id, refundAmount);
                transactionMessage =
                    $"Pending character refunded and deleted. ({refundAmount} PT gained | {guild.GetPlayerTokenCount(user.Id)} PT remaining)";
            }
            await charThread.DeleteAsync();

            await transactionChannel.SendMessageAsync(transactionMessage);
        }
    }
}
