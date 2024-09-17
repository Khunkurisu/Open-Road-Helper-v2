using OpenRoadHelper.Characters;
using OpenRoadHelper.Guilds;
using Discord;
using Discord.WebSocket;

namespace OpenRoadHelper
{
    public partial class Manager
    {
        public static async Task SendAvatarPrompt(SocketMessageComponent component)
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
            IUser user = component.User;
            if (user.Id != playerId)
            {
                await InteractionErrors.MissingPermissions(component, user);
                return;
            }

            string charName = formValues.Target;
            Character? character = GetCharacter(guildId, playerId, charName);
            if (character == null)
            {
				await InteractionErrors.CharacterNotFound(component, charName);
                return;
            }

            IThreadChannel? charThread = (IThreadChannel?)GetTextChannel(
                character.Guild,
                character.CharacterThread
            );
            if (charThread == null)
            {
                await InteractionErrors.CharacterThreadNotFound(component, charName);
                return;
            }

            character.AwaitingInput = DateTime.UtcNow;

            await component.RespondAsync(
                $"Please upload avatar images as attachments in {charThread.Mention}",
                ephemeral: true
            );
        }

        public static async Task ShiftAvatar(SocketMessageComponent component)
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
            IUser user = component.User;
            if (user.Id != playerId)
            {
                await InteractionErrors.MissingPermissions(component, user);
                return;
            }

            string charName = formValues.Target;
            Character? character = GetCharacter(guildId, playerId, charName);
            if (character == null)
            {
				await InteractionErrors.CharacterNotFound(component, charName);
                return;
            }

            string directionToShift = formValues.Modifier;
            if (directionToShift == "forward")
            {
                character.NextAvatar();
                await Task.Yield();
                await Guild.RefreshCharacterPosts(character);
                guild.QueueSave(SaveType.Characters);
            }
            else if (directionToShift == "back")
            {
                character.PreviousAvatar();
                await Task.Yield();
                await Guild.RefreshCharacterPosts(character);
                guild.QueueSave(SaveType.Characters);
            }
            await component.UpdateAsync(x => x.Content = x.Content);
        }
    }
}
