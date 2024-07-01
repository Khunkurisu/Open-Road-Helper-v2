using Bot.Characters;
using Bot.Guilds;
using Discord;
using Discord.WebSocket;

namespace Bot
{
    public partial class BotManager
    {
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
            List<SocketMessageComponentData> components
        )
        {
            string guildId = modal.Data.CustomId.Split("+")[1];
            Guild guild = GetGuild(ulong.Parse(guildId));
            string playerIdString = modal.Data.CustomId.Split("+")[2];
            ulong playerId = ulong.Parse(playerIdString);
            IUser user = modal.User;
            IUser? player = _client?.GetUser(playerId);
            if (player != null)
            {
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
                        if (tokenCount < tokenCost)
                        {
                            await modal.RespondAsync(
                                "You lack sufficient tokens to create a new character. "
                                    + $"({tokenCost} PT required - {tokenCount} PT remaining.)"
                            );
                        }
                        else
                        {
                            charData["name"] = charName;
                            charData["description"] = charDescription;
                            charData["reputation"] = charReputation;
                            Character character = new(player, guild, charData);
                            guild.AddCharacter(user.Id, character);

                            await modal.RespondAsync(
                                "Ensure everything looks correct before continuing!",
                                embed: character.GenerateEmbed(user).Build(),
                                components: character.GenerateComponents(guild.Id, user.Id).Build()
                            );
                        }
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
            else if (user.Id != playerId)
            {
                await modal.RespondAsync();
            }
            else
            {
                await modal.RespondAsync("something went terribly wrong");
            }
        }

        private static async Task CharacterCreateConfirm(SocketMessageComponent button)
        {
            IUser user = button.User;
            string player = button.Data.CustomId.Split("+")[2];
            if ("" + user.Id == player)
            {
                string guildId = button.Data.CustomId.Split("+")[1];
                Guild guild = GetGuild(ulong.Parse(guildId));

                string charName = button.Data.CustomId.Split("+")[3];
                int tokenCost = guild.GetNewCharacterCost(user.Id);
                Console.WriteLine($"token cost: {tokenCost}");
                uint tokenCount = guild.GetPlayerTokenCount(user.Id);
                Console.WriteLine($"token count: {tokenCount}");

                Character? character = guild.GetCharacter(user.Id, charName);
                if (character != null)
                {
                    guild.DecreasePlayerTokenCount(user.Id, tokenCost);
                    character.Status = Status.Pending;
                    await PostCharacter(button, guild, character, user);
                }
                else
                {
                    await button.UpdateAsync(x =>
                    {
                        x.Content = "Somehow character was null.";
                    });
                }
            }
            else
            {
                await button.UpdateAsync(x => { });
            }
        }

        private static async Task CharacterCreateCancel(SocketMessageComponent button)
        {
            IUser user = button.User;
            string player = button.Data.CustomId.Split("+")[2];
            if ("" + user.Id == player)
            {
                string guildId = button.Data.CustomId.Split("+")[1];

                Guild guild = GetGuild(ulong.Parse(guildId));

                guild.ClearTempCharacter(user.Id);
                string charName = button.Data.CustomId.Split("+")[3];
                guild.RemoveCharacter(user.Id, charName);
                await button.UpdateAsync(x =>
                {
                    x.Content = "Character creation canceled.";
                    x.Embed = null;
                    x.Components = null;
                });
            }
            else
            {
                await button.UpdateAsync(x => { });
            }
        }

        private static async Task PostCharacter(
            SocketMessageComponent context,
            Guild guild,
            Character character,
            IUser user
        )
        {
            if (guild.CharacterBoard != null)
            {
                guild.ClearTempCharacter(user.Id);
                ForumTag[] tags =
                {
                    guild.CharacterBoard.Tags.First(x => x.Name == "Pending Approval")
                };
                IThreadChannel thread = await guild.CharacterBoard.CreatePostAsync(
                    character.Name,
                    ThreadArchiveDuration.OneWeek,
                    embed: character.GenerateEmbed(user).Build(),
                    tags: tags,
                    components: character.GenerateComponents(guild.Id, user.Id).Build()
                );
                character.Thread = thread.Id;
                await thread.AddUserAsync((IGuildUser)user);
                await context.UpdateAsync(x =>
                {
                    x.Content = "Character posted! " + thread.Mention;
                    x.Embed = null;
                    x.Components = null;
                });
            }
            else
            {
                await context.UpdateAsync(x =>
                {
                    x.Content = "Character board has not been assigned.";
                });
            }
        }
    }
}
