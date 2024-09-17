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
            bool updateDisplay = false,
            bool isForced = false
        )
        {
            if (component.GuildId == null)
            {
				await InteractionErrors.MustRunInGuild(component, component.User);
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
                await InteractionErrors.MissingPermissions(component, user);
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
                    guild.QueueSave(SaveType.Characters);
                }
            }

            await component.UpdateAsync(x =>
            {
                x.Embed = character.GenerateEmbed(user).Build();
                x.Components = character.GenerateComponents(new(), isForced).Build();
            });
        }

        public static async Task DrawCharacterPost(Character character, SocketInteraction context)
        {
            IThreadChannel? charThread = (IThreadChannel?)GetTextChannel(
                character.Guild,
                character.CharacterThread
            );
            if (charThread == null)
            {
                await context.RespondAsync(
                    $"Character thread for {character.Name} could not be located."
                );
                return;
            }
            IUser? user = GetGuildUser(character.Guild, character.User);
            if (user == null)
            {
				await InteractionErrors.UserNotFound(context, character.User);
                return;
            }
            await charThread.ModifyMessageAsync(
                character.CharacterThread,
                msg =>
                {
                    msg.Embed = character.GenerateEmbed(user).Build();
                    msg.Components = character.GenerateComponents(new()).Build();
                }
            );
        }

        public static async Task DrawCharacterPost(Character character)
        {
            IThreadChannel? charThread = (IThreadChannel?)GetTextChannel(
                character.Guild,
                character.CharacterThread
            );
            if (charThread == null)
            {
                return;
            }
            IUser? user = GetGuildUser(character.Guild, character.User);
            if (user == null)
            {
                return;
            }
            await charThread.ModifyMessageAsync(
                character.CharacterThread,
                msg =>
                {
                    msg.Embed = character.GenerateEmbed(user).Build();
                    msg.Components = character.GenerateComponents(new()).Build();
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
