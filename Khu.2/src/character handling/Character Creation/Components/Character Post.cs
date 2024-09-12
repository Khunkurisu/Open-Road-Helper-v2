using OpenRoadHelper.Guilds;
using Discord;

namespace OpenRoadHelper.Characters
{
    public partial class Character
    {
        public ComponentBuilder GenerateComponents(List<string> metadata, bool isForced = false)
        {
            return GenerateButtons(metadata, isForced)
                .WithSelectMenu(CharacterDisplaySelector(isForced));
        }

        public static SelectMenuBuilder CharacterDisplaySelector(
            ulong guildId,
            ulong userId,
            string charName
        )
        {
            Guild guild = Manager.GetGuild(guildId);

            return new SelectMenuBuilder()
                .WithCustomId(
                    guild.GenerateFormValues(
                        new($"charDisplay", userId, charName, string.Empty, new())
                    )
                )
                .AddOption("Details", "Details", isDefault: true)
                .AddOption("Attributes", "Attributes")
                .AddOption("Equipment", "Equipment")
                .AddOption("Spells", "Spells")
                .AddOption("Feats", "Feats");
        }

        public SelectMenuBuilder CharacterDisplaySelector(bool isForced = false)
        {
            Guild guild = Manager.GetGuild(Guild);

            var menu = new SelectMenuBuilder().WithCustomId(
                guild.GenerateFormValues(
                    new($"charDisplay", User, Name, isForced ? "forced" : "not forced", new())
                )
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
                .WithCustomId(
                    guild.GenerateFormValues(
                        new($"retirementType", User, Name, string.Empty, new())
                    )
                )
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
                    guild.GenerateFormValues(new($"retireCharacter", User, Name, "confirm", new())),
                    ButtonStyle.Success,
                    row: 1
                )
                .WithButton(
                    "Cancel",
                    guild.GenerateFormValues(new($"retireCharacter", User, Name, "cancel", new())),
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
