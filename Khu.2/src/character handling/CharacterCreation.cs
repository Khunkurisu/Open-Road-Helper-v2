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

        /* private async Task CharacterCreate(
            SocketModal modal,
            List<SocketMessageComponentData> components
        ) { } */

        private static async Task CharacterCreate(
            SocketModal modal,
            List<SocketMessageComponentData> components
        )
        {
            string guildId = modal.Data.CustomId.Split("-")[1];
            Guild guild = GetGuild(ulong.Parse(guildId));
            string playerIdString = modal.Data.CustomId.Split("-")[2];
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
                        int tokenCost = guild.GetPlayerCharacterCount(playerId) + 1;
                        uint tokenCount = guild.GetPlayerTokenCount(playerId);
                        if (tokenCount < tokenCost)
                        {
                            await modal.RespondAsync(
                                "You lack sufficient tokens to create a new character. ("
                                    + tokenCost
                                    + " PT required)"
                            );
                        }
                        else
                        {
                            Character character =
                                new(
                                    player,
                                    guild,
                                    charName,
                                    charDescription,
                                    charReputation,
                                    charData
                                );

                            /* guild.SetPlayerTokenCount(playerId, (uint)(tokenCount - tokenCost));
                            guild.AddCharacter(playerId, character); */
                            EmbedBuilder? charEmbed = character.GenerateEmbed();
                            if (charEmbed != null)
                            {
                                await modal.RespondAsync(
                                    "Ensure everything looks correct before continuing!",
                                    embed: charEmbed.Build()
                                );
                            }
                            else
                            {
                                await modal.RespondAsync("failed to create embed");
                            }
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
    }
}
