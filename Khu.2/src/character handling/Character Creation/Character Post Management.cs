using Bot.Characters;
using Bot.Guilds;
using Discord;
using Discord.WebSocket;

namespace Bot
{
    public partial class Manager
    {
        public static async Task DrawCharacterPost(SocketMessageComponent messageComponent)
        {
            string guildId = messageComponent.Data.CustomId.Split("+")[1];
            Guild guild = GetGuild(ulong.Parse(guildId));

            string userId = messageComponent.Data.CustomId.Split("+")[2];
            IUser? user = GetGuildUser(guild.Id, ulong.Parse(userId));
            if (user != null)
            {
                string charName = messageComponent.Data.CustomId.Split("+")[3];
                Character? character = GetCharacter(guild.Id, user.Id, charName);
                if (character != null)
                {
                    if (messageComponent.Data.CustomId.Contains("charDisplay"))
                    {
                        string displayName = string.Join(", ", messageComponent.Data.Values);
                        if (Enum.TryParse(displayName, out Character.Display displayValue))
                        {
                            character.DisplayMode = displayValue;
                        }
                    }
                    await messageComponent.UpdateAsync(x =>
                    {
                        x.Embed = character.GenerateEmbed(user).Build();
                        x.Components = character.GenerateComponents().Build();
                    });
                }
            }
        }

        public static async Task DrawCharacterPost(Character character, SocketInteraction context)
        {
            IThreadChannel? threadChannel = GetThreadChannel(
                character.Guild,
                character.CharacterThread
            );
            if (threadChannel == null)
            {
                await context.RespondAsync(
                    $"Character thread for {character.Name} could not be located."
                );
                return;
            }
            IUser? user = GetGuildUser(character.Guild, character.User);
            if (user == null)
            {
                await context.RespondAsync(
                    $"User <@{character.User}> could not be found in database."
                );
                return;
            }
            await threadChannel.ModifyMessageAsync(
                character.CharacterThread,
                msg =>
                {
                    msg.Embed = character.GenerateEmbed(user).Build();
                    msg.Components = character.GenerateComponents().Build();
                }
            );
        }

        public static async Task DrawCharacterPost(Character character)
        {
            IThreadChannel? threadChannel = GetThreadChannel(
                character.Guild,
                character.CharacterThread
            );
            if (threadChannel == null)
            {
                return;
            }
            IUser? user = GetGuildUser(character.Guild, character.User);
            if (user == null)
            {
                return;
            }
            await threadChannel.ModifyMessageAsync(
                character.CharacterThread,
                msg =>
                {
                    msg.Embed = character.GenerateEmbed(user).Build();
                    msg.Components = character.GenerateComponents().Build();
                    msg.Content = string.Empty;
                }
            );
        }

        private static async Task CharacterEditStart(SocketMessageComponent button)
        {
            IUser user = button.User;
            string player = button.Data.CustomId.Split("+")[2];
            if (user.Id.ToString() == player)
            {
                string guildId = button.Data.CustomId.Split("+")[1];
                Guild guild = GetGuild(ulong.Parse(guildId));

                guild.ClearTempCharacter(user.Id);
                await button.UpdateAsync(x =>
                {
                    x.Content = "Character edit canceled.";
                    x.Embed = null;
                    x.Components = null;
                });
            }
            else
            {
                await button.UpdateAsync(x => { });
            }
        }

        private static async Task CharacterEditCancel(SocketMessageComponent button)
        {
            IUser user = button.User;
            string player = button.Data.CustomId.Split("+")[2];
            if (user.Id.ToString() == player)
            {
                string guildId = button.Data.CustomId.Split("+")[1];
                Guild guild = GetGuild(ulong.Parse(guildId));

                await button.UpdateAsync(x =>
                {
                    x.Content = "Character update canceled.";
                    x.Embed = null;
                    x.Components = null;
                });
            }
            else
            {
                await button.UpdateAsync(x => { });
            }
        }

        private static async Task CharacterEditConfirm(SocketMessageComponent button)
        {
            IUser user = button.User;
            string player = button.Data.CustomId.Split("+")[2];
            if (user.Id.ToString() == player)
            {
                string guildId = button.Data.CustomId.Split("+")[1];
                Guild guild = GetGuild(ulong.Parse(guildId));

                await button.UpdateAsync(x =>
                {
                    x.Content = "Character update canceled.";
                    x.Embed = null;
                    x.Components = null;
                });
            }
            else
            {
                await button.UpdateAsync(x => { });
            }
        }
    }
}
