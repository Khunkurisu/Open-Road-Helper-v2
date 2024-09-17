using OpenRoadHelper.Characters;
using OpenRoadHelper.Guilds;
using Discord;
using Discord.WebSocket;

namespace OpenRoadHelper
{
    public partial class Manager
    {
        public static async Task RetireCharacter(
            SocketMessageComponent component,
            bool updateRetirementStatus = false
        )
        {
            if (component.GuildId == null)
            {
				await InteractionErrors.MustRunInGuild(component, component.User);
                return;
            }
            ulong guildId = (ulong)component.GuildId;
            Guild guild = GetGuild(guildId);

            FormValue formValues = guild.GetFormValues(component.Data.CustomId);

            ulong playerId = formValues.User;
            IUser? player = GetGuildUser(guildId, playerId);
            if (player == null)
            {
                await component.RespondAsync(
                    $"Unable to find <@{playerId}> in database.",
                    ephemeral: true
                );
                return;
            }

            IUser user = component.User;
            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await InteractionErrors.MissingPermissions(component, user);
                return;
            }

            string charName = formValues.Target;
            Character? character = guild.GetCharacter(playerId, charName);
            if (character == null)
            {
				await InteractionErrors.CharacterNotFound(component, charName);
                return;
            }
            if (character.Status != Status.Approved)
            {
                await component.RespondAsync(
                    $"{charName} could not be retired due to incorrect status.",
                    ephemeral: true
                );
                return;
            }

            if (updateRetirementStatus)
            {
                string statusName = string.Join(", ", component.Data.Values);
                if (Enum.TryParse(statusName, out Status statusValue))
                {
                    character.RetirementType = statusValue;
                }

                await component.UpdateAsync(x =>
                {
                    x.Content = $"What end has {charName} met?";
                    x.Components = character.RetirementPost().Build();
                });
            }
            else
            {
                await component.RespondAsync(
                    $"What end has {charName} met?",
                    ephemeral: true,
                    components: character.RetirementPost().Build()
                );
            }
        }

        public static async Task RetireCharacterConfirm(SocketMessageComponent component)
        {
            if (component.GuildId == null)
            {
				await InteractionErrors.MustRunInGuild(component, component.User);
                return;
            }
            ulong guildId = (ulong)component.GuildId;
            Guild guild = GetGuild(guildId);

            FormValue formValues = guild.GetFormValues(component.Data.CustomId);

            ulong playerId = formValues.User;
            IUser? player = GetGuildUser(guildId, playerId);
            if (player == null)
            {
                await component.RespondAsync(
                    $"Unable to find <@{playerId}> in database.",
                    ephemeral: true
                );
                return;
            }

            IUser user = component.User;
            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await InteractionErrors.MissingPermissions(component, user);
                return;
            }

            string charName = formValues.Target;
            Character? character = guild.GetCharacter(playerId, charName);
            if (character == null)
            {
				await InteractionErrors.CharacterNotFound(component, charName);
                return;
            }
            if (character.Status != Status.Approved)
            {
                await component.RespondAsync(
                    $"{charName} could not be retired due to incorrect status.",
                    ephemeral: true
                );
                return;
            }

            IThreadChannel? charThread = (IThreadChannel?)GetTextChannel(
                character.Guild,
                character.CharacterThread
            );
            IThreadChannel? transactionChannel = (IThreadChannel?)GetTextChannel(
                guild.Id,
                character.TransactionThread
            );
            if (transactionChannel == null || charThread == null)
            {
                await InteractionErrors.CharacterThreadNotFound(component, charName);
                return;
            }

            character.Status = character.RetirementType;
            guild.RemoveCharacter(character);
            uint refundAmount = (uint)guild.GetNewCharacterCost(playerId);
            guild.IncreasePlayerTokenCount(playerId, refundAmount);
            guild.AddCharacter(playerId, character);
            if (guild.CharacterBoard == null)
            {
                await component.RespondAsync(
                    $"Server forum boards have not been configured.",
                    ephemeral: true
                );
                return;
            }
            ulong[] tags =
            {
                guild.CharacterBoard.Tags.First(x => x.Name == character.Status.ToString()).Id
            };
            await charThread.ModifyAsync(x => x.AppliedTags = tags);

            IUserMessage msg = await transactionChannel.SendMessageAsync(
                $"{charName} marked as {character.RetirementType}. ({refundAmount} PT gained | {guild.GetPlayerTokenCount(playerId)} PT remaining)"
            );
            string msgLink = $"https://discord.com/channels/{guildId}/{msg.Channel.Id}/{msg.Id}";

            await component.UpdateAsync(x =>
            {
                x.Content = $"{charName} marked as {character.RetirementType}. ({msgLink})";
                x.Embed = null;
                x.Components = null;
            });
            await Guild.RefreshCharacterPosts(character);
        }

        public static async Task RetireCharacterCancel(SocketMessageComponent component)
        {
            if (component.GuildId == null)
            {
				await InteractionErrors.MustRunInGuild(component, component.User);
                return;
            }
            ulong guildId = (ulong)component.GuildId;
            Guild guild = GetGuild(guildId);

            FormValue formValues = guild.GetFormValues(component.Data.CustomId);

            ulong playerId = formValues.User;
            IUser? player = GetGuildUser(guildId, playerId);
            if (player == null)
            {
                await component.RespondAsync(
                    $"Unable to find <@{playerId}> in database.",
                    ephemeral: true
                );
                return;
            }

            IUser user = component.User;
            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await InteractionErrors.MissingPermissions(component, user);
                return;
            }

            string charName = formValues.Target;
            await component.UpdateAsync(x =>
            {
                x.Content = $"{charName} retirement canceled.";
                x.Embed = null;
                x.Components = null;
            });
        }
    }
}
