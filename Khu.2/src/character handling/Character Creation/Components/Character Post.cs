using Bot.Guilds;
using Bot.Helpers;
using Discord;

namespace Bot.Characters
{
    public partial class Character
    {
        public ComponentBuilder GenerateComponents(bool isForced = false)
        {
            return GenerateButtons(isForced).WithSelectMenu(CharacterDisplaySelector());
        }

        public static SelectMenuBuilder CharacterDisplaySelector(
            ulong guildId,
            ulong userId,
            string charName
        )
        {
            Guild guild = Manager.GetGuild(guildId);

            return new SelectMenuBuilder()
                .WithCustomId(guild.GenerateFormValues(new() { $"charDisplay", userId, charName }))
                .AddOption("Details", "Details", isDefault: true)
                .AddOption("Attributes", "Attributes")
                .AddOption("Equipment", "Equipment")
                .AddOption("Spells", "Spells")
                .AddOption("Feats", "Feats");
        }

        public SelectMenuBuilder CharacterDisplaySelector()
        {
            Guild guild = Manager.GetGuild(Guild);

            var menu = new SelectMenuBuilder().WithCustomId(
                guild.GenerateFormValues(new() { $"charDisplay", User, Name })
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
