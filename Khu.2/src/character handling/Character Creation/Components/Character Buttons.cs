using OpenRoadHelper.Guilds;
using Discord;

namespace OpenRoadHelper.Characters
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
                        new($"{context}", playerId, charName, "confirm", new())
                    ),
                    ButtonStyle.Success
                )
                .WithButton(
                    "Cancel",
                    guild.GenerateFormValues(
                        new($"{context}", playerId, charName, "cancel", new())
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

        public ComponentBuilder PowerTokenButtons()
        {
            return PowerTokenButtons(new());
        }

        public ComponentBuilder PowerTokenButtons(ComponentBuilder button)
        {
            Guild guild = Manager.GetGuild(Guild);

            return button
                .WithButton(
                    "PT2GP",
                    guild.GenerateFormValues(new("pt2gp", User, Name, "gp", new())),
                    ButtonStyle.Primary,
                    disabled: !CanTradePT2GP()
                )
                .WithButton(
                    "PT2DT",
                    guild.GenerateFormValues(new("pt2dt", User, Name, "dt", new())),
                    ButtonStyle.Primary,
                    disabled: !CanTradePT2DT()
                );
        }

        public ComponentBuilder QuantityButtons(
            string context,
            List<string> metadata,
            List<bool> disabledButtons
        )
        {
            Guild guild = Manager.GetGuild(Guild);

            return new ComponentBuilder()
                .WithButton(
                    "-5",
                    guild.GenerateFormValues(
                        new($"decrement{context}", User, Name, "-5", metadata)
                    ),
                    disabled: disabledButtons[0]
                )
                .WithButton(
                    "-1",
                    guild.GenerateFormValues(
                        new($"decrement{context}", User, Name, "-1", metadata)
                    ),
                    disabled: disabledButtons[1]
                )
                .WithButton(
                    "+1",
                    guild.GenerateFormValues(
                        new($"increment{context}", User, Name, "+1", metadata)
                    ),
                    disabled: disabledButtons[2]
                )
                .WithButton(
                    "+5",
                    guild.GenerateFormValues(
                        new($"increment{context}", User, Name, "+5", metadata)
                    ),
                    disabled: disabledButtons[3]
                );
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
                    guild.GenerateFormValues(new($"shiftAvatar", User, Name, "back", new())),
                    ButtonStyle.Primary,
                    row: 1,
                    disabled: CheckMoreAvatars(false)
                )
                .WithButton(
                    "Next Avatar",
                    guild.GenerateFormValues(new($"shiftAvatar", User, Name, "forward", new())),
                    ButtonStyle.Primary,
                    row: 1,
                    disabled: CheckMoreAvatars()
                );
        }

        public ComponentBuilder? GenerateButtons()
        {
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
                guild.GenerateFormValues(new($"editCharacter", User, Name, "init", new())),
                ButtonStyle.Primary
            );

            if (isForced)
            {
                if (Status == Status.Pending)
                {
                    buttons
                        .WithButton(
                            "Judge",
                            guild.GenerateFormValues(
                                new($"judgeCharacter", User, Name, "init", new())
                            ),
                            ButtonStyle.Primary
                        )
                        .WithButton(
                            "Refund",
                            guild.GenerateFormValues(
                                new($"refundCharacterFromForced", User, Name, "init", new())
                            ),
                            ButtonStyle.Danger
                        );
                }
                else if (Status == Status.Rejected)
                {
                    buttons.WithButton(
                        "Refund",
                        guild.GenerateFormValues(
                            new($"refundCharacterFromForced", User, Name, "init", new())
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
                            guild.GenerateFormValues(
                                new($"judgeCharacter", User, Name, "init", new())
                            ),
                            ButtonStyle.Primary
                        )
                        .WithButton(
                            "Refund",
                            guild.GenerateFormValues(
                                new($"refundCharacter", User, Name, "init", new())
                            ),
                            ButtonStyle.Danger
                        );
                }
                else if (Status == Status.Rejected)
                {
                    buttons.WithButton(
                        "Refund",
                        guild.GenerateFormValues(
                            new($"refundCharacter", User, Name, "init", new())
                        ),
                        ButtonStyle.Danger
                    );
                }
                else if (Status == Status.Approved)
                {
                    buttons.WithButton(
                        "Retire",
                        guild.GenerateFormValues(
                            new($"retireCharacter", User, Name, "init", new())
                        ),
                        ButtonStyle.Danger
                    );
                    buttons = PowerTokenButtons(buttons);
                    buttons = AvatarButtons(buttons);
                }
            }

            return buttons;
        }

        public ComponentBuilder GenerateButtons(bool isForced = false)
        {
            if (!isForced && Status == Status.Temp)
            {
                return ConfirmationButtons(Guild, User, Name, "createCharacter");
            }
            if (isForced && Status == Status.Temp)
            {
                return ConfirmationButtons(Guild, User, Name, "forceCreateCharacter");
            }

            return GenerateButtons(new(), isForced);
        }
    }
}
