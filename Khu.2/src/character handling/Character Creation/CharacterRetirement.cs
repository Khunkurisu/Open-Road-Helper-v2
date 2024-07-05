using Bot.Characters;
using Bot.Guilds;
using Discord;
using Discord.WebSocket;

namespace Bot
{
    public partial class Manager
    {
        public static async Task RetireCharacter(
            SocketMessageComponent component,
            bool updateRetirementStatus = false
        )
        {
            if (component.GuildId == null)
            {
                await component.RespondAsync("This must be run in a guild.", ephemeral: true);
                return;
            }
            ulong guildId = (ulong)component.GuildId;
            Guild guild = GetGuild(guildId);

            List<dynamic> formValues = guild.GetFormValues(component.Data.CustomId);

            ulong playerId = formValues[1];
            IUser user = component.User;
            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await component.RespondAsync("You lack permission to do that!", ephemeral: true);
                return;
            }

            string charName = formValues[2];
            Character? character = guild.GetCharacter(user.Id, charName);
            if (character == null)
            {
                await component.RespondAsync(
                    $"{charName} could not be found in database for {user.Username}.",
                    ephemeral: true
                );
                return;
            }
            if (character.Status != Status.Approved)
            {
                await component.RespondAsync(
                    $"{charName} could not be retired due to incorrect status.",
                    ephemeral: true
                );
                return;
            }

            if (updateRetirementStatus)
            {
                string statusName = string.Join(", ", component.Data.Values);
                if (Enum.TryParse(statusName, out Status statusValue))
                {
                    character.RetirementType = statusValue;
                }

                await component.UpdateAsync(x =>
                {
                    x.Content = $"What end has {charName} met?";
                    x.Components = character.RetirementPost().Build();
                });
            }
            else
            {
                await component.RespondAsync(
                    $"What end has {charName} met?",
                    ephemeral: true,
                    components: character.RetirementPost().Build()
                );
            }
        }

        public static async Task RetireCharacterConfirm(SocketMessageComponent component)
        {
            if (component.GuildId == null)
            {
                await component.RespondAsync("This must be run in a guild.", ephemeral: true);
                return;
            }
            ulong guildId = (ulong)component.GuildId;
            Guild guild = GetGuild(guildId);

            List<dynamic> formValues = guild.GetFormValues(component.Data.CustomId);

            ulong playerId = formValues[1];
            IUser user = component.User;
            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await component.RespondAsync("You lack permission to do that!", ephemeral: true);
                return;
            }

            string charName = formValues[2];
            Character? character = guild.GetCharacter(user.Id, charName);
            if (character == null)
            {
                await component.RespondAsync(
                    $"{charName} could not be found in database for {user.Username}.",
                    ephemeral: true
                );
                return;
            }
            if (character.Status != Status.Approved)
            {
                await component.RespondAsync(
                    $"{charName} could not be retired due to incorrect status.",
                    ephemeral: true
                );
                return;
            }
        }

        public static async Task RetireCharacterCancel(SocketMessageComponent component)
        {
            if (component.GuildId == null)
            {
                await component.RespondAsync("This must be run in a guild.", ephemeral: true);
                return;
            }
            ulong guildId = (ulong)component.GuildId;
            Guild guild = GetGuild(guildId);

            List<dynamic> formValues = guild.GetFormValues(component.Data.CustomId);

            ulong playerId = formValues[1];
            IUser user = component.User;
            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await component.RespondAsync("You lack permission to do that!", ephemeral: true);
                return;
            }

            string charName = formValues[2];
            Character? character = guild.GetCharacter(user.Id, charName);
            if (character == null)
            {
                await component.RespondAsync(
                    $"{charName} could not be found in database for {user.Username}.",
                    ephemeral: true
                );
                return;
            }
        }
    }
}
