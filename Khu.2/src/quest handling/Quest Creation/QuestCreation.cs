using OpenRoadHelper.Guilds;
using OpenRoadHelper.Quests;
using Discord;
using Discord.WebSocket;

namespace OpenRoadHelper
{
    public partial class Manager
    {
        private static async Task QuestCreateConfirm(SocketMessageComponent component)
        {
            if (component.GuildId == null)
            {
				await InteractionErrors.MustRunInGuild(component, component.User);
                return;
            }

            ulong guildId = (ulong)component.GuildId;
            Guild guild = GetGuild(guildId);
            FormValue formValues = guild.GetFormValues(component.Data.CustomId);

            ulong gmId = formValues.User;
            IUser? gm = GetGuildUser(guildId, gmId);
            if (gm == null)
            {
                await component.RespondAsync("Unable to find user.", ephemeral: true);
                return;
            }

            IUser user = component.User;
            if (user.Id != gmId && !guild.IsGamemaster(user))
            {
                await InteractionErrors.MissingPermissions(component, user);
                return;
            }

            string questName = formValues.Target;
            Quest? quest = guild.GetQuest(questName, gm);
            if (quest == null)
            {
                await component.UpdateAsync(x =>
                {
                    x.Content = $"Unable to locate {questName}.";
                    x.Embed = null;
                    x.Components = null;
                });
                return;
            }

            if (quest.Status != Status.Unplayed)
            {
                await component.UpdateAsync(x =>
                {
                    x.Content = $"{questName} has an invalid status for that action.";
                    x.Embed = null;
                    x.Components = null;
                });
                return;
            }

            await PostQuest(component, guild, quest, gm);
        }

        private static async Task QuestCreateCancel(SocketMessageComponent component)
        {
            if (component.GuildId == null)
            {
				await InteractionErrors.MustRunInGuild(component, component.User);
                return;
            }

            ulong guildId = (ulong)component.GuildId;
            Guild guild = GetGuild(guildId);

            FormValue formValues = guild.GetFormValues(component.Data.CustomId);

            ulong gmId = formValues.User;
            IUser user = component.User;
            if (user.Id != gmId && !guild.IsGamemaster(user))
            {
                await InteractionErrors.MissingPermissions(component, user);
                return;
            }

            string questName = formValues.Target;
            Quest? quest = guild.GetQuest(questName, user);
            if (quest == null)
            {
                await component.UpdateAsync(x =>
                {
                    x.Content = $"Unable to locate {questName}.";
                    x.Embed = null;
                    x.Components = null;
                });
                return;
            }
            if (quest.Status != Status.Unplayed)
            {
                await component.UpdateAsync(x =>
                {
                    x.Content = $"{questName} has an invalid status for that action.";
                    x.Embed = null;
                    x.Components = null;
                });
                return;
            }

            guild.RemoveQuest(quest);
            await component.UpdateAsync(x =>
            {
                x.Content = "Quest creation canceled.";
                x.Embed = null;
                x.Components = null;
            });
        }

        private static async Task PostQuest(
            SocketMessageComponent context,
            Guild guild,
            Quest quest,
            IUser user
        )
        {
            if (guild.BoardsUnset())
            {
                await context.UpdateAsync(x =>
                {
                    x.Content = $"Server forum boards have not been configured.";
                    x.Embed = null;
                    x.Components = null;
                });
                return;
            }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            ForumTag[] tags = { guild.QuestBoard.Tags.First(x => x.Name == "Pending") };
            quest.Status = Status.Unplayed;

            IThreadChannel questThread = await guild.QuestBoard.CreatePostAsync(
                quest.Name,
                ThreadArchiveDuration.OneWeek,
                embed: quest.GenerateEmbed(user).Build(),
                tags: tags,
                components: quest.GenerateComponents().Build()
            );

            IThreadChannel transThread = await guild.TransactionBoard.CreatePostAsync(
                quest.Name,
                ThreadArchiveDuration.OneWeek,
                text: $"{quest.Name} Transactions"
            );
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            await transThread.SendMessageAsync(
                $"{questThread.Mention} has been created and is pending approval."
            );

            quest.ThreadId = questThread.Id;
            guild.QueueSave(SaveType.Quests);

            await questThread.AddUserAsync((IGuildUser)user);
            await transThread.AddUserAsync((IGuildUser)user);

            await context.FollowupAsync(
                $"{quest.Name} has been created and is pending approval. (Quest: {questThread.Mention} | Transactions: {transThread.Mention})",
                embeds: null,
                components: null
            );
        }
    }
}
