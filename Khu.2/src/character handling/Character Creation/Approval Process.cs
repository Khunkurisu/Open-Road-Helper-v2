using Bot.Characters;
using Bot.Guilds;
using Discord;
using Discord.WebSocket;

namespace Bot
{
    public partial class Manager
    {
        private static async Task JudgeCharacter(SocketMessageComponent messageComponent)
        {
            string guildId = messageComponent.Data.CustomId.Split("+")[1];
            Guild guild = GetGuild(ulong.Parse(guildId));

            string userId = messageComponent.Data.CustomId.Split("+")[2];
            IUser? user = GetGuildUser(guild.Id, ulong.Parse(userId));
            if (user == null)
            {
                await messageComponent.RespondAsync(
                    $"<@{userId}> could not be found in database.",
                    ephemeral: true
                );
                return;
            }
            if (!IsGamemaster(user, guild.Id))
            {
                await messageComponent.RespondAsync(
                    "Only Game Masters may access this feature.",
                    ephemeral: true
                );
                return;
            }
            string charName = messageComponent.Data.CustomId.Split("+")[3];
            Character? character = GetCharacter(guild.Id, user.Id, charName);
            if (character == null)
            {
                await messageComponent.RespondAsync(
                    $"{charName} could not be found in database.",
                    ephemeral: true
                );
                return;
            }
            await messageComponent.RespondAsync(
                $"Would you like to approve {character.Name} or send a note of rejection?",
                components: ApproveRejectButtons(guildId, user.Id, charName, "judgeCharacter")
                    .Build(),
                ephemeral: true
            );
        }

        private static async Task ApproveCharacter(SocketMessageComponent messageComponent)
        {
            string guildId = messageComponent.Data.CustomId.Split("+")[1];
            Guild guild = GetGuild(ulong.Parse(guildId));
            if (guild.CharacterBoard == null || guild.TransactionBoard == null)
            {
                await messageComponent.RespondAsync(
                    "Server forum boards have not been assigned.",
                    ephemeral: true
                );
                return;
            }

            string userId = messageComponent.Data.CustomId.Split("+")[2];
            IUser? user = GetGuildUser(guild.Id, ulong.Parse(userId));
            if (user == null)
            {
                await messageComponent.RespondAsync(
                    $"<@{userId}> could not be found in database.",
                    ephemeral: true
                );
                return;
            }

            if (!IsGamemaster(user, guild.Id))
            {
                await messageComponent.RespondAsync(
                    "Only Game Masters may access this feature.",
                    ephemeral: true
                );
                return;
            }

            string charName = messageComponent.Data.CustomId.Split("+")[3];
            Character? character = GetCharacter(guild.Id, user.Id, charName);
            if (character == null)
            {
                await messageComponent.RespondAsync(
                    $"{charName} could not be found in database.",
                    ephemeral: true
                );
                return;
            }

            IThreadChannel? threadChannel = GetThreadChannel(guild.Id, character.CharacterThread);
            if (threadChannel == null)
            {
                await messageComponent.RespondAsync(
                    $"Character thread for {charName} could not be found.",
                    ephemeral: true
                );
                return;
            }

            IThreadChannel? transactions = GetThreadChannel(guild.Id, character.TransactionThread);
            if (transactions == null)
            {
                await messageComponent.RespondAsync(
                    $"Thread for transactions could not be found.",
                    ephemeral: true
                );
                return;
            }

            character.Status = Status.Approved;
            guild.QueueSave("characters");

            var msg = await transactions.SendMessageAsync(
                $"{threadChannel.Mention} has been approved."
            );
            string msgLink = $"https://discord.com/channels/{guildId}/{msg.Channel.Id}/{msg.Id}";
            await messageComponent.UpdateAsync(x =>
            {
                x.Content = $"{charName} has been approved. ({msgLink})";
                x.Components = null;
            });

            ulong[] tags = { guild.CharacterBoard.Tags.First(x => x.Name == "Approved").Id };
            await threadChannel.ModifyAsync(x =>
            {
                x.AppliedTags = tags;
            });
            await DrawCharacterPost(character, messageComponent);
        }

        private static async Task RejectCharacter(SocketMessageComponent messageComponent)
        {
            string guildId = messageComponent.Data.CustomId.Split("+")[1];
            Guild guild = GetGuild(ulong.Parse(guildId));

            if (guild.CharacterBoard == null && guild.TransactionBoard == null)
            {
                await messageComponent.RespondAsync(
                    "Server forum boards have not been assigned.",
                    ephemeral: true
                );
            }

            string userId = messageComponent.Data.CustomId.Split("+")[2];
            IUser? user = GetGuildUser(guild.Id, ulong.Parse(userId));
            if (user == null)
            {
                await messageComponent.RespondAsync(
                    $"<@{userId}> could not be found in database.",
                    ephemeral: true
                );
                return;
            }

            if (!IsGamemaster(user, guild.Id))
            {
                await messageComponent.RespondAsync(
                    "Only Game Masters may access this feature.",
                    ephemeral: true
                );
                return;
            }

            string charName = messageComponent.Data.CustomId.Split("+")[3];

            var modal = new ModalBuilder()
                .WithTitle("Reject Character")
                .WithCustomId($"rejectCharacter+{guildId}+{userId}+{charName}")
                .AddTextInput(
                    "Reason",
                    "rejection_reason",
                    TextInputStyle.Paragraph,
                    "Detail the reasoning for this rejection."
                );

            await messageComponent.DeleteOriginalResponseAsync();
            await messageComponent.RespondWithModalAsync(modal.Build());
        }

        private static async Task RejectCharacter(
            SocketModal modal,
            List<SocketMessageComponentData> components
        )
        {
            string guildId = modal.Data.CustomId.Split("+")[1];
            Guild guild = GetGuild(ulong.Parse(guildId));
            if (guild.CharacterBoard != null && guild.TransactionBoard != null)
            {
                string userId = modal.Data.CustomId.Split("+")[2];
                IUser? user = GetGuildUser(guild.Id, ulong.Parse(userId));
                if (user == null)
                {
                    await modal.RespondAsync(
                        $"<@{userId}> could not be found in database.",
                        ephemeral: true
                    );
                    return;
                }

                if (!IsGamemaster(user, guild.Id))
                {
                    await modal.RespondAsync(
                        "Only Game Masters may access this feature.",
                        ephemeral: true
                    );
                    return;
                }

                string charName = modal.Data.CustomId.Split("+")[3];
                Character? character = GetCharacter(guild.Id, user.Id, charName);
                if (character == null)
                {
                    await modal.RespondAsync(
                        $"{charName} could not be found in database.",
                        ephemeral: true
                    );
                    return;
                }

                IThreadChannel? threadChannel = GetThreadChannel(
                    guild.Id,
                    character.CharacterThread
                );
                if (threadChannel == null)
                {
                    await modal.RespondAsync(
                        $"Character thread for {charName} could not be found.",
                        ephemeral: true
                    );
                    return;
                }

                IThreadChannel? transactions = GetThreadChannel(
                    guild.Id,
                    character.TransactionThread
                );
                if (transactions == null)
                {
                    await modal.RespondAsync(
                        $"Thread for transactions could not be found.",
                        ephemeral: true
                    );
                    return;
                }

                character.Status = Status.Rejected;
                guild.QueueSave("characters");

                ulong[] tags = { guild.CharacterBoard.Tags.First(x => x.Name == "Rejected").Id };

                await threadChannel.ModifyAsync(x =>
                {
                    x.AppliedTags = tags;
                });

                string reason = components.First(x => x.CustomId == "rejection_reason").Value;

                var msg = await transactions.SendMessageAsync(
                    $"{threadChannel.Mention} has been rejected.",
                    embed: new EmbedBuilder()
                        .WithTitle("Rejection Reason")
                        .WithDescription(reason)
                        .WithAuthor(user.Username)
                        .Build()
                );
                string msgLink =
                    $"https://discord.com/channels/{guildId}/{msg.Channel.Id}/{msg.Id}";
                await modal.RespondAsync(
                    $"{charName} has been rejected. ({msgLink})",
                    ephemeral: true
                );
                await DrawCharacterPost(character, modal);
            }
        }

        public static ComponentBuilder ApproveRejectButtons(
            string guildId,
            ulong playerId,
            string charName,
            string context
        )
        {
            Guild guild = GetGuild(ulong.Parse(guildId));

            return new ComponentBuilder()
                .WithButton(
                    "Approve",
                    guild.GenerateFormValues(
                        new() { $"{context}Character", playerId, charName, "approve" }
                    ),
                    ButtonStyle.Success
                )
                .WithButton(
                    "Reject",
                    guild.GenerateFormValues(
                        new() { $"{context}Character", playerId, charName, "reject" }
                    ),
                    ButtonStyle.Danger
                );
        }
    }
}
