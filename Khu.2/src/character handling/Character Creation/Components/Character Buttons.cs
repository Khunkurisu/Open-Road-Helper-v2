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
            Guild guild = Manager.GetGuild(_guild);
            IUser? player = Manager.GetGuildUser(_guild, _user);
            if (player == null)
            {
                return null;
            }

            return GenerateButtons(guild.Id, player.Id);
        }

        public ComponentBuilder GenerateButtons(
            ulong guildId,
            ulong playerId,
            bool isForced = false
        )
        {
            if (!isForced && Status == Status.Temp)
            {
                return ConfirmationButtons("" + guildId, playerId, Name, "create");
            }
            if (isForced && Status == Status.Temp)
            {
                return ConfirmationButtons("" + guildId, playerId, Name, "forceCreate");
            }

            var buttons = new ComponentBuilder().WithButton(
                "Edit",
                "editCharacter+" + guildId + "+" + playerId + "+" + Name,
                ButtonStyle.Primary
            );

            if (isForced)
            {
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
                            "refundCharacterFromForced+" + guildId + "+" + playerId + "+" + Name,
                            ButtonStyle.Danger
                        );
                }
                else if (Status == Status.Rejected)
                {
                    buttons.WithButton(
                        "Refund",
                        "refundCharacterFromForced+" + guildId + "+" + playerId + "+" + Name,
                        ButtonStyle.Danger
                    );
                }
            }
            else
            {
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
                else if (Status == Status.Rejected)
                {
                    buttons.WithButton(
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
            }

            return buttons;
        }
    }
}
