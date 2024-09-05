using OpenRoadHelper.Characters;
using OpenRoadHelper.Guilds;
using Discord;
using Discord.WebSocket;

namespace OpenRoadHelper
{
    public partial class Manager
    {
        public static async Task GainGoldFromPT(SocketMessageComponent button)
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

            await character.BuyGold(button);
        }

        public static async Task ModifyGoldFromPT(SocketMessageComponent button)
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
            await character.ModifyGoldFromPT(button);
        }

        public static async Task ConfirmBuyGold(SocketMessageComponent button)
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
            await character.ConfirmBuyGold(button);
        }

        public static async Task GainDowntimeFromPT(SocketMessageComponent button)
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

            await character.BuyDowntime(button);
        }

        public static async Task ModifyDowntimeFromPT(SocketMessageComponent button)
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
            await character.ModifyDowntimeFromPT(button);
        }

        public static async Task ConfirmBuyDowntime(SocketMessageComponent button)
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
            await character.ConfirmBuyDowntime(button);
        }
    }
}
