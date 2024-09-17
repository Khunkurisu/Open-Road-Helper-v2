using Discord;
using Discord.WebSocket;

namespace OpenRoadHelper
{
    public static class InteractionErrors
    {
        public static async Task UserNotFound(SocketMessageComponent component, ulong userId)
        {
            await component.RespondAsync(
                $"User <@{userId}> could not be found in the database.",
                ephemeral: true
            );
            Logger.Warn($"User <@{userId}> could not be found in the database.");
        }

        public static async Task UserNotFound(SocketInteraction component, ulong userId)
        {
            await component.RespondAsync(
                $"User <@{userId}> could not be found in the database.",
                ephemeral: true
            );
            Logger.Warn($"User <@{userId}> could not be found in the database.");
        }

        public static async Task CharacterNotFound(
            SocketMessageComponent component,
            string charName
        )
        {
            await component.RespondAsync(
                $"{charName} could not be found in database.",
                ephemeral: true
            );
            Logger.Warn($"{charName} could not be found in database.");
        }

        public static async Task CharacterNotFound(SocketInteraction component, string charName)
        {
            await component.RespondAsync(
                $"{charName} could not be found in database.",
                ephemeral: true
            );
            Logger.Warn($"{charName} could not be found in database.");
        }

        public static async Task CharacterThreadNotFound(
            SocketInteraction component,
            string charName
        )
        {
            await component.RespondAsync(
                $"{charName} forum threads could not be found.",
                ephemeral: true
            );
            Logger.Warn($"{charName} forum threads could not be found.");
        }

        public static async Task CharacterThreadNotFound(string charName)
        {
            Logger.Warn($"{charName} forum threads could not be found.");
            await Task.CompletedTask;
        }

        public static async Task CharacterThreadNotFound(
            SocketMessageComponent component,
            string charName,
            bool update = false
        )
        {
            if (!update)
            {
                await component.RespondAsync(
                    $"{charName} forum threads could not be found.",
                    ephemeral: true
                );
            }
            else
            {
                await component.UpdateAsync(x =>
                {
                    x.Content = $"{charName} forum threads could not be found.";
                    x.Components = null;
                    x.Embed = null;
                });
            }
            Logger.Warn($"{charName} forum threads could not be found.");
        }

        public static async Task MissingPermissions(SocketMessageComponent component, IUser user)
        {
            await component.RespondAsync("This must be run in a guild.", ephemeral: true);
            Logger.Warn(
                $"User ({user.Username}|<{user.Id}>) attempted component use while lacking permission to do so."
            );
        }

        public static async Task MissingPermissions(SocketInteraction component, IUser user)
        {
            await component.RespondAsync("This must be run in a guild.", ephemeral: true);
            Logger.Warn(
                $"User ({user.Username}|<{user.Id}>) attempted component use while lacking permission to do so."
            );
        }

        public static async Task MustRunInGuild(SocketMessageComponent component, IUser user)
        {
            await component.RespondAsync("This must be run in a guild.", ephemeral: true);
            Logger.Warn(
                $"User ({user.Username}|<{user.Id}>) attempted component use outside guild."
            );
        }

        public static async Task MustRunInGuild(SocketInteraction component, IUser user)
        {
            await component.RespondAsync("This must be run in a guild.", ephemeral: true);
            Logger.Warn(
                $"User ({user.Username}|<{user.Id}>) attempted component use outside guild."
            );
        }
    }
}
