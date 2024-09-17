using OpenRoadHelper.Characters;
using OpenRoadHelper.Guilds;
using Discord;
using Discord.WebSocket;

namespace OpenRoadHelper
{
    public partial class Manager
    {
        public static async Task SpendPTPrompt(SocketMessageComponent component)
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

            await component.RespondAsync(
                "How would you like to spend your PT?",
                components: character.PowerTokenButtons().Build(),
                ephemeral: true
            );
        }

        public static async Task GainGoldFromPT(SocketMessageComponent component)
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

            await character.BuyGold(component);
        }

        public static async Task ModifyGoldFromPT(SocketMessageComponent component)
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
            await character.ModifyGoldFromPT(component);
        }

        public static async Task ConfirmBuyGold(SocketMessageComponent component)
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
            await character.ConfirmBuyGold(component);
        }

        public static async Task GainDowntimeFromPT(SocketMessageComponent component)
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

            await character.BuyDowntime(component);
        }

        public static async Task ModifyDowntimeFromPT(SocketMessageComponent component)
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
            await character.ModifyDowntimeFromPT(component);
        }

        public static async Task ConfirmBuyDowntime(SocketMessageComponent component)
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
            await character.ConfirmBuyDowntime(component);
        }
    }
}
