using OpenRoadHelper.Characters;
using OpenRoadHelper.Guilds;
using Discord;
using Discord.WebSocket;

namespace OpenRoadHelper
{
    public partial class Manager
    {
        private static async Task JudgeCharacter(SocketMessageComponent component)
        {
            if (component.GuildId == null)
            {
                await InteractionErrors.MustRunInGuild(component, component.User);
                return;
            }
            ulong guildId = (ulong)component.GuildId;
            Guild guild = GetGuild(guildId);

            FormValue formValue = guild.GetFormValues(component.Data.CustomId);

            ulong playerId = formValue.User;
            IUser? player = GetGuildUser(guild.Id, playerId);
            if (player == null)
            {
                await InteractionErrors.UserNotFound(component, playerId);
                return;
            }

            IUser user = component.User;
            if (!IsGamemaster(user, guild.Id))
            {
                await component.RespondAsync(
                    "Only Game Masters may access this feature.",
                    ephemeral: true
                );
                return;
            }
            string charName = formValue.Target;
            Character? character = GetCharacter(guildId, playerId, charName);
            if (character == null)
            {
                await InteractionErrors.CharacterNotFound(component, charName);
                return;
            }
            await component.RespondAsync(
                $"Would you like to approve {character.Name} or send a note of rejection?",
                components: ApproveRejectButtons(guildId, player.Id, charName, "judgeCharacter")
                    .Build(),
                ephemeral: true
            );
        }

        private static async Task ApproveCharacter(SocketMessageComponent component)
        {
            if (component.GuildId == null)
            {
                await InteractionErrors.MustRunInGuild(component, component.User);
                return;
            }
            ulong guildId = (ulong)component.GuildId;
            Guild guild = GetGuild(guildId);
            if (guild.CharacterBoard == null || guild.TransactionBoard == null)
            {
                await component.RespondAsync(
                    "Server forum boards have not been assigned.",
                    ephemeral: true
                );
                return;
            }

            FormValue formValue = guild.GetFormValues(component.Data.CustomId);

            ulong playerId = formValue.User;
            IUser? player = GetGuildUser(guild.Id, playerId);
            if (player == null)
            {
                await InteractionErrors.UserNotFound(component, playerId);
                return;
            }

            IUser user = component.User;
            if (!IsGamemaster(user, guild.Id))
            {
                await component.RespondAsync(
                    "Only Game Masters may access this feature.",
                    ephemeral: true
                );
                return;
            }

            string charName = formValue.Target;
            Character? character = GetCharacter(guildId, playerId, charName);
            if (character == null)
            {
                await InteractionErrors.CharacterNotFound(component, charName);
                return;
            }

            IThreadChannel? charThread = (IThreadChannel?)GetTextChannel(
                character.Guild,
                character.CharacterThread
            );
            if (charThread == null)
            {
                await InteractionErrors.CharacterThreadNotFound(component, charName);
                return;
            }

            IThreadChannel? transactions = GetThreadChannel(guild.Id, character.TransactionThread);
            if (transactions == null)
            {
                await InteractionErrors.CharacterThreadNotFound(component, charName);
                return;
            }

            character.Status = Status.Approved;
            guild.QueueSave(SaveType.Characters);

            var msg = await transactions.SendMessageAsync(
                $"{charThread.Mention} has been approved."
            );
            string msgLink = $"https://discord.com/channels/{guildId}/{msg.Channel.Id}/{msg.Id}";
            await component.UpdateAsync(x =>
            {
                x.Content = $"{charName} has been approved. ({msgLink})";
                x.Components = null;
            });

            ulong[] tags = { guild.CharacterBoard.Tags.First(x => x.Name == "Approved").Id };
            await charThread.ModifyAsync(x =>
            {
                x.AppliedTags = tags;
            });
            await DrawCharacterPost(character, component);
        }

        private static async Task RejectCharacter(SocketMessageComponent component)
        {
            if (component.GuildId == null)
            {
                await InteractionErrors.MustRunInGuild(component, component.User);
                return;
            }
            ulong guildId = (ulong)component.GuildId;
            Guild guild = GetGuild(guildId);
            if (guild.CharacterBoard == null || guild.TransactionBoard == null)
            {
                await component.RespondAsync(
                    "Server forum boards have not been assigned.",
                    ephemeral: true
                );
                return;
            }

            FormValue formValue = guild.GetFormValues(component.Data.CustomId);

            ulong playerId = formValue.User;
            IUser? player = GetGuildUser(guild.Id, playerId);
            if (player == null)
            {
                await InteractionErrors.UserNotFound(component, playerId);
                return;
            }

            IUser user = component.User;
            if (!IsGamemaster(user, guild.Id))
            {
                await component.RespondAsync(
                    "Only Game Masters may access this feature.",
                    ephemeral: true
                );
                return;
            }

            string charName = formValue.Target;

            var modal = new ModalBuilder()
                .WithTitle("Reject Character")
                .WithCustomId(
                    guild.GenerateFormValues(
                        new($"rejectCharacter", playerId, charName, string.Empty, new())
                    )
                )
                .AddTextInput(
                    "Reason",
                    "rejection_reason",
                    TextInputStyle.Paragraph,
                    "Detail the reasoning for this rejection."
                );

            await component.DeleteOriginalResponseAsync();
            await component.RespondWithModalAsync(modal.Build());
        }

        private static async Task RejectCharacter(
            SocketModal modal,
            List<SocketMessageComponentData> components
        )
        {
            if (modal.GuildId == null)
            {
                await InteractionErrors.MustRunInGuild(modal, modal.User);
                return;
            }
            ulong guildId = (ulong)modal.GuildId;
            Guild guild = GetGuild(guildId);
            if (guild.CharacterBoard == null || guild.TransactionBoard == null)
            {
                await modal.RespondAsync(
                    "Server forum boards have not been assigned.",
                    ephemeral: true
                );
                return;
            }

            FormValue formValue = guild.GetFormValues(modal.Data.CustomId);

            ulong playerId = formValue.User;
            IUser? player = GetGuildUser(guild.Id, playerId);
            if (player == null)
            {
                await InteractionErrors.UserNotFound(modal, playerId);
                return;
            }

            IUser user = modal.User;
            if (!IsGamemaster(user, guild.Id))
            {
                await modal.RespondAsync(
                    "Only Game Masters may access this feature.",
                    ephemeral: true
                );
                return;
            }

            string charName = formValue.Target;
            Character? character = GetCharacter(guildId, playerId, charName);
            if (character == null)
            {
                await InteractionErrors.CharacterNotFound(modal, charName);
                return;
            }

            IThreadChannel? charThread = (IThreadChannel?)GetTextChannel(
                character.Guild,
                character.CharacterThread
            );
            if (charThread == null)
            {
                await InteractionErrors.CharacterThreadNotFound(modal, charName);
                return;
            }

            IThreadChannel? transactions = GetThreadChannel(guild.Id, character.TransactionThread);
            if (transactions == null)
            {
                await InteractionErrors.CharacterThreadNotFound(modal, charName);
                return;
            }

            character.Status = Status.Rejected;
            guild.QueueSave(SaveType.Characters);

            ulong[] tags = { guild.CharacterBoard.Tags.First(x => x.Name == "Rejected").Id };

            await charThread.ModifyAsync(x =>
            {
                x.AppliedTags = tags;
            });

            string reason = components.First(x => x.CustomId == "rejection_reason").Value;

            var msg = await transactions.SendMessageAsync(
                $"{charThread.Mention} has been rejected.",
                embed: new EmbedBuilder()
                    .WithTitle("Rejection Reason")
                    .WithDescription(reason)
                    .WithAuthor(player.Username)
                    .Build()
            );
            string msgLink = $"https://discord.com/channels/{guildId}/{msg.Channel.Id}/{msg.Id}";
            await modal.RespondAsync($"{charName} has been rejected. ({msgLink})", ephemeral: true);
            await DrawCharacterPost(character, modal);
        }

        public static ComponentBuilder ApproveRejectButtons(
            ulong guildId,
            ulong playerId,
            string charName,
            string context
        )
        {
            Guild guild = GetGuild(guildId);

            return new ComponentBuilder()
                .WithButton(
                    "Approve",
                    guild.GenerateFormValues(
                        new($"{context}Character", playerId, charName, "approve", new())
                    ),
                    ButtonStyle.Success
                )
                .WithButton(
                    "Reject",
                    guild.GenerateFormValues(
                        new($"{context}Character", playerId, charName, "reject", new())
                    ),
                    ButtonStyle.Danger
                );
        }
    }
}
