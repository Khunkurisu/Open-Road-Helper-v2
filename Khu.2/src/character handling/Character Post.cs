using Bot.Guilds;
using Bot.Helpers;
using Bot.PF2;
using Discord;

namespace Bot.Characters
{
    public partial class Character
    {
        private Color GetDiscordColor()
        {
            int[] colorValues = GenericHelpers.HexStringToRGB(_colorPref);
            return new Color(colorValues[0], colorValues[1], colorValues[2]);
        }

        public static ComponentBuilder ConfirmationButtons(
            string guildId,
            ulong playerId,
            string charName,
            string context
        )
        {
            return new ComponentBuilder()
                .WithButton(
                    "Confirm",
                    context + "Character+" + guildId + "+" + playerId + "+" + charName + "+confirm",
                    ButtonStyle.Success
                )
                .WithButton(
                    "Cancel",
                    context + "Character+" + guildId + "+" + playerId + "+" + charName + "+cancel",
                    ButtonStyle.Danger
                );
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

        public ComponentBuilder? GenerateButtons()
        {
            Guild guild = BotManager.GetGuild(_guild);
            IUser? player = BotManager.GetGuildUser(_guild, _user);
            if (player == null)
            {
                return null;
            }

            return GenerateButtons(guild.Id, player.Id);
        }

        public ComponentBuilder GenerateButtons(ulong guildId, ulong playerId)
        {
            if (Status == Status.Temp)
            {
                return ConfirmationButtons("" + guildId, playerId, Name, "create");
            }

            var buttons = new ComponentBuilder().WithButton(
                "Edit",
                "editCharacter+" + guildId + "+" + playerId + "+" + Name,
                ButtonStyle.Primary
            );

            if (Status == Status.Pending)
            {
                buttons
                    .WithButton(
                        "Judge",
                        "judgeCharacter+" + guildId + "+" + playerId + "+" + Name,
                        ButtonStyle.Primary
                    )
                    .WithButton(
                        "Refund",
                        "refundCharacter+" + guildId + "+" + playerId + "+" + Name,
                        ButtonStyle.Danger
                    );
            }
            else if (Status == Status.Approved)
            {
                buttons.WithButton(
                    "Retire",
                    "retireCharacter+" + guildId + "+" + playerId + "+" + Name,
                    ButtonStyle.Danger
                );
            }

            return buttons;
        }

        public ComponentBuilder GenerateComponents(ulong guildId, ulong playerId)
        {
            return GenerateButtons(guildId, playerId)
                .WithSelectMenu(CharacterDisplaySelector(guildId, playerId));
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
