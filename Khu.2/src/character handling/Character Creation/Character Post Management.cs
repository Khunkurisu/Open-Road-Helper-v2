using OpenRoadHelper.Characters;
using OpenRoadHelper.Guilds;
using Discord;
using Discord.WebSocket;

namespace OpenRoadHelper
{
    public partial class Manager
    {
        public static async Task DrawCharacterPost(
            SocketMessageComponent component,
            bool updateDisplay = false, bool isForced = false
        )
        {
            if (component.GuildId == null)
            {
                await component.RespondAsync("This must be run in a guild.", ephemeral: true);
                return;
            }
            ulong guildId = (ulong)component.GuildId;
            Guild guild = GetGuild(guildId);

            FormValue formValues = guild.GetFormValues(component.Data.CustomId);

            ulong playerId = formValues.User;
            IUser? player = GetGuildUser(guildId, playerId);
            if (player == null)
            {
                await component.RespondAsync(
                    $"Unable to find <@{playerId}> in database.",
                    ephemeral: true
                );
                return;
            }

            IUser user = component.User;
            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await component.RespondAsync("You lack permission to do that!", ephemeral: true);
                return;
            }

            if (player != user && !guild.IsGamemaster(user))
            {
                await component.RespondAsync(
                    $"You lack permission to perform that action.",
                    ephemeral: true
                );
                return;
            }

            string charName = formValues.Target;
            Character? character = guild.GetCharacter(playerId, charName);
            if (character == null)
            {
                await component.RespondAsync(
                    $"Unable to locate {charName} for user <@{player.Id}>.",
                    ephemeral: true
                );
                return;
            }

            if (updateDisplay)
            {
                string displayName = string.Join(", ", component.Data.Values);
                if (Enum.TryParse(displayName, out Character.Display displayValue))
                {
                    character.DisplayMode = displayValue;
                    guild.QueueSave("characters");
                }
            }

            await component.UpdateAsync(x =>
            {
                x.Embed = character.GenerateEmbed(user).Build();
                x.Components = character.GenerateComponents(isForced).Build();
            });
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
            await button.RespondAsync("WIP.", ephemeral: true);
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
