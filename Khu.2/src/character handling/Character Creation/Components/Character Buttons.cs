using Bot.Guilds;
using Discord;

namespace Bot.Characters
{
    public partial class Character
    {
        public static ComponentBuilder ConfirmationButtons(
            ulong guildId,
            ulong playerId,
            string charName,
            string context
        )
        {
            Guild guild = Manager.GetGuild(guildId);

            return new ComponentBuilder()
                .WithButton(
                    "Confirm",
                    guild.GenerateFormValues(
                        new() { $"{context}Character", playerId, charName, "confirm" }
                    ),
                    ButtonStyle.Success
                )
                .WithButton(
                    "Cancel",
                    guild.GenerateFormValues(
                        new() { $"{context}Character", playerId, charName, "cancel" }
                    ),
                    ButtonStyle.Danger
                );
        }

        public static ComponentBuilder ConfirmationButtons(
            string guildId,
            ulong playerId,
            string charName,
            string context
        )
        {
            return ConfirmationButtons(ulong.Parse(guildId), playerId, charName, context);
        }

        public ComponentBuilder AvatarButtons()
        {
            return AvatarButtons(new ComponentBuilder());
        }

        public ComponentBuilder AvatarButtons(ComponentBuilder button)
        {
            Guild guild = Manager.GetGuild(Guild);

            return button
                .WithButton(
                    "Previous Avatar",
                    guild.GenerateFormValues(new() { $"shiftAvatar", User, Name, "back" }),
                    ButtonStyle.Primary,
                    row: 1,
                    disabled: CheckMoreAvatars(false)
                )
                .WithButton(
                    "Next Avatar",
                    guild.GenerateFormValues(new() { $"shiftAvatar", User, Name, "forward" }),
                    ButtonStyle.Primary,
                    row: 1,
                    disabled: CheckMoreAvatars()
                );
        }

        public ComponentBuilder? GenerateButtons()
        {
            Guild guild = Manager.GetGuild(Guild);
            IUser? player = Manager.GetGuildUser(Guild, User);
            if (player == null)
            {
                return null;
            }

            return GenerateButtons();
        }

        public ComponentBuilder GenerateButtons(ComponentBuilder buttons, bool isForced = false)
        {
            Guild guild = Manager.GetGuild(Guild);

            buttons.WithButton(
                "Edit",
                "editCharacter+" + Guild + "+" + User + "+" + Name,
                ButtonStyle.Primary
            );

            if (isForced)
            {
                if (Status == Status.Pending)
                {
                    buttons
                        .WithButton(
                            "Judge",
                            guild.GenerateFormValues(new() { $"judgeCharacter", User, Name }),
                            ButtonStyle.Primary
                        )
                        .WithButton(
                            "Refund",
                            guild.GenerateFormValues(
                                new() { $"refundCharacterFromForced", User, Name }
                            ),
                            ButtonStyle.Danger
                        );
                }
                else if (Status == Status.Rejected)
                {
                    buttons.WithButton(
                        "Refund",
                        guild.GenerateFormValues(
                            new() { $"refundCharacterFromForced", User, Name }
                        ),
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
                            guild.GenerateFormValues(new() { $"judgeCharacter", User, Name }),
                            ButtonStyle.Primary
                        )
                        .WithButton(
                            "Refund",
                            guild.GenerateFormValues(new() { $"refundCharacter", User, Name }),
                            ButtonStyle.Danger
                        );
                }
                else if (Status == Status.Rejected)
                {
                    buttons.WithButton(
                        "Refund",
                        guild.GenerateFormValues(new() { $"refundCharacter", User, Name }),
                        ButtonStyle.Danger
                    );
                }
                else if (Status == Status.Approved)
                {
                    buttons.WithButton(
                        "Retire",
                        guild.GenerateFormValues(new() { $"retireCharacter", User, Name }),
                        ButtonStyle.Danger
                    );
                    buttons = AvatarButtons(buttons);
                }
            }

            return buttons;
        }

        public ComponentBuilder GenerateButtons(bool isForced = false)
        {
            if (!isForced && Status == Status.Temp)
            {
                return ConfirmationButtons(Guild, User, Name, "create");
            }
            if (isForced && Status == Status.Temp)
            {
                return ConfirmationButtons(Guild, User, Name, "forceCreate");
            }

            return GenerateButtons(new(), isForced);
        }
    }
}
