using Bot.Characters;
using Bot.Guilds;
using Discord;
using Discord.WebSocket;

namespace Bot
{
    public partial class Manager
    {
        public static async Task RetireCharacter(SocketMessageComponent button)
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
            if (character.Status != Status.Approved)
            {
                await button.RespondAsync(
                    $"{charName} could not be retired due to incorrect status.",
                    ephemeral: true
                );
                return;
            }
        }

        public static SelectMenuBuilder RetirementTypeSelector(Guild guild, Character character)
        {
            var menu = new SelectMenuBuilder()
                .WithCustomId(
                    guild.GenerateFormValues(
                        new() { $"retirementType", character.User, character.Name }
                    )
                )
                .AddOption("Retired", "Retired")
                .AddOption("Deceased", "Deceased");

            return menu;
        }
    }
}
