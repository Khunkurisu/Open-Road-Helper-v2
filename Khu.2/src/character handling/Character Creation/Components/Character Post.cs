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

        public SelectMenuBuilder RetirementTypeSelector()
        {
            Guild guild = Manager.GetGuild(Guild);

            var menu = new SelectMenuBuilder()
                .WithCustomId(guild.GenerateFormValues(new() { $"retirementType", User, Name }))
                .AddOption(
                    Status.Retired.ToString(),
                    Status.Retired.ToString(),
                    isDefault: RetirementType == Status.Retired
                )
                .AddOption(
                    Status.Deceased.ToString(),
                    Status.Deceased.ToString(),
                    isDefault: RetirementType == Status.Deceased
                );

            return menu;
        }

        public ComponentBuilder RetirementPost()
        {
            Guild guild = Manager.GetGuild(Guild);

            return new ComponentBuilder()
                .WithSelectMenu(RetirementTypeSelector())
                .WithButton(
                    "Confirm",
                    guild.GenerateFormValues(new() { $"retireCharacter", User, Name, "confirm" }),
                    ButtonStyle.Success,
                    row: 1
                )
                .WithButton(
                    "Cancel",
                    guild.GenerateFormValues(new() { $"retireCharacter", User, Name, "cancel" }),
                    ButtonStyle.Danger,
                    row: 1
                );
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
