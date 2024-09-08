using OpenRoadHelper.Characters;
using OpenRoadHelper.Guilds;
using Discord;
using Discord.WebSocket;

namespace OpenRoadHelper
{
    public partial class Manager
    {
        public static async Task SendAvatarPrompt(SocketMessageComponent button)
        {
            if (button.GuildId == null)
            {
                await button.RespondAsync("This must be run in a guild.", ephemeral: true);
                return;
            }
            ulong guildId = (ulong)button.GuildId;
            Guild guild = GetGuild(guildId);

            FormValue formValues = guild.GetFormValues(button.Data.CustomId);

            ulong playerId = formValues.User;
            IUser user = button.User;
            if (user.Id != playerId)
            {
                await button.RespondAsync("You lack permission to do that!", ephemeral: true);
                return;
            }

            string charName = formValues.Target;
            Character? character = GetCharacter(guildId, playerId, charName);
            if (character == null)
            {
                await button.RespondAsync(
                    $"{charName} could not be found in database.",
                    ephemeral: true
                );
                return;
            }

            IThreadChannel? charThread = (IThreadChannel?)GetTextChannel(
                character.Guild,
                character.CharacterThread
            );
            if (charThread == null)
            {
                await button.RespondAsync(
                    $"{character.Name} forum threads could not be found.",
                    ephemeral: true
                );
                return;
            }

            character.AwaitingInput = DateTime.UtcNow;

            await button.RespondAsync(
                $"Please upload avatar images as attachments in {charThread.Mention}",
                ephemeral: true
            );
        }

        public static async Task ShiftAvatar(SocketMessageComponent button)
        {
            if (button.GuildId == null)
            {
                await button.RespondAsync("This must be run in a guild.", ephemeral: true);
                return;
            }
            ulong guildId = (ulong)button.GuildId;
            Guild guild = GetGuild(guildId);

            FormValue formValues = guild.GetFormValues(button.Data.CustomId);

            ulong playerId = formValues.User;
            IUser user = button.User;
            if (user.Id != playerId)
            {
                await button.RespondAsync("You lack permission to do that!", ephemeral: true);
                return;
            }

            string charName = formValues.Target;
            Character? character = GetCharacter(guildId, playerId, charName);
            if (character == null)
            {
                await button.RespondAsync(
                    $"{charName} could not be found in database.",
                    ephemeral: true
                );
                return;
            }

            string directionToShift = formValues.Modifier;
            if (directionToShift == "forward")
            {
                character.NextAvatar();
                await Task.Yield();
                await guild.RefreshCharacterPosts(button.User.Id);
                guild.QueueSave(SaveType.Characters);
            }
            else if (directionToShift == "back")
            {
                character.PreviousAvatar();
                await Task.Yield();
                await guild.RefreshCharacterPosts(button.User.Id);
                guild.QueueSave(SaveType.Characters);
            }
            await button.UpdateAsync(x => x.Content = x.Content);
        }
    }
}
