using Bot.Guilds;
using Bot.Helpers;
using Discord;

namespace Bot.Characters
{
    public partial class Character
    {
        public ComponentBuilder GenerateComponents(ulong guildId, ulong playerId)
        {
            return GenerateButtons(guildId, playerId)
                .WithSelectMenu(CharacterDisplaySelector(guildId, playerId));
        }

        public static SelectMenuBuilder CharacterDisplaySelector(
            ulong guildId,
            ulong userId,
            string name
        )
        {
            return new SelectMenuBuilder()
                .WithCustomId("charDisplay+" + guildId + "+" + userId + "+" + name)
                .AddOption("Details", "Details", isDefault: true)
                .AddOption("Attributes", "Attributes")
                .AddOption("Equipment", "Equipment")
                .AddOption("Spells", "Spells")
                .AddOption("Feats", "Feats");
        }

        public SelectMenuBuilder CharacterDisplaySelector(ulong guildId, ulong userId)
        {
            var menu = new SelectMenuBuilder().WithCustomId(
                "charDisplay+" + guildId + "+" + userId + "+" + Name
            );

            foreach (string k in Enum.GetNames(typeof(Display)))
            {
                menu.AddOption(k, k, isDefault: DisplayMode.ToString() == k);
            }

            return menu;
        }

        public enum Display
        {
            Details,
            Attributes,
            Equipment,
            Spells,
            Feats
        }
    }
}
