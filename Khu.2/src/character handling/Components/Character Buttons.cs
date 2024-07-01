using Bot.Guilds;
using Bot.Helpers;
using Bot.PF2;
using Discord;

namespace Bot.Characters
{
    public partial class Character
    {
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
    }
}
