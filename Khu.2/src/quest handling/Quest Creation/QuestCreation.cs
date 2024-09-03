using OpenRoadHelper.Guilds;
using OpenRoadHelper.Quests;
using Discord;
using Discord.WebSocket;

namespace OpenRoadHelper
{
    public partial class Manager
    {
        private static async Task QuestCreateConfirm(
            SocketMessageComponent button
        )
        {
            if (button.GuildId == null)
            {
                await button.RespondAsync("This must be run in a guild.", ephemeral: true);
                return;
            }

            ulong guildId = (ulong)button.GuildId;
            Guild guild = GetGuild(guildId);
            FormValue formValues = guild.GetFormValues(button.Data.CustomId);

            ulong gmId = formValues.User;
            IUser? gm = GetGuildUser(guildId, gmId);
            if (gm == null)
            {
                await button.RespondAsync("Unable to find user.", ephemeral: true);
                return;
            }

            IUser user = button.User;
            if (user.Id != gmId && !guild.IsGamemaster(user))
            {
                await button.RespondAsync("You lack permission to do that!", ephemeral: true);
                return;
            }

            string questName = formValues.Target;
            Quest? quest = guild.GetQuest(questName, gm);
            if (quest == null)
            {
                await button.UpdateAsync(x =>
                {
                    x.Content = $"Unable to locate {questName}.";
                    x.Embed = null;
                    x.Components = null;
                });
                return;
            }

            if (quest.Status != Status.Unplayed)
            {
                await button.UpdateAsync(x =>
                {
                    x.Content = $"{questName} has an invalid status for that action.";
                    x.Embed = null;
                    x.Components = null;
                });
                return;
            }

            await PostQuest(button, guild, quest, gm);
        }

        private static async Task QuestCreateCancel(SocketMessageComponent button)
        {
            if (button.GuildId == null)
            {
                await button.RespondAsync("This must be run in a guild.", ephemeral: true);
                return;
            }

            ulong guildId = (ulong)button.GuildId;
            Guild guild = GetGuild(guildId);

            FormValue formValues = guild.GetFormValues(button.Data.CustomId);

            ulong gmId = formValues.User;
            IUser gm = button.User;
            if (gm.Id != gmId && !guild.IsGamemaster(gm))
            {
                await button.RespondAsync("You lack permission to do that!", ephemeral: true);
                return;
            }

            string questName = formValues.Target;
            Quest? quest = guild.GetQuest(questName, gm);
            if (quest == null)
            {
                await button.UpdateAsync(x =>
                {
                    x.Content = $"Unable to locate {questName}.";
                    x.Embed = null;
                    x.Components = null;
                });
                return;
            }
            if (quest.Status != Status.Unplayed)
            {
                await button.UpdateAsync(x =>
                {
                    x.Content = $"{questName} has an invalid status for that action.";
                    x.Embed = null;
                    x.Components = null;
                });
                return;
            }

            guild.RemoveQuest(quest);
            await button.UpdateAsync(x =>
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

            await transThread.SendMessageAsync(
                $"{questThread.Mention} has been created and is pending approval."
            );

            quest.ThreadId = questThread.Id;
            guild.QueueSave("quests");

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
