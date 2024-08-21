using Bot.Characters;
using Bot.Guilds;
using Discord;
using Discord.Interactions;
using Newtonsoft.Json;

namespace Bot.GameMaster
{
    [Group("admin", "Perform admin related tasks with these commands.")]
    public partial class AdminManagement : InteractionModuleBase<SocketInteractionContext>
    {
        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [Group("refresh", "Refresh posts for the given list.")]
        public class RefreshListing : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("characters", "Refresh all character posts with database values.")]
            public async Task RefreshCharacterPosts()
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync(
                        "Only GMs may run this command!",
                        ephemeral: true
                    );
                    return;
                }
                await guild.RefreshCharacterPosts(Context);
                await Context.Interaction.RespondAsync("Character posts refreshed!");
            }

            [SlashCommand("quests", "Refresh all character posts with database values.")]
            public async Task RefreshQuestPosts()
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync(
                        "Only GMs may run this command!",
                        ephemeral: true
                    );
                    return;
                }
                await guild.RefreshQuestPosts();
                await Context.Interaction.RespondAsync("Character posts refreshed!");
            }
        }

        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [Group("gm-roles", "Manage roles assigned as Game Master.")]
        public class GameMasterRoles : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("add", "Add a role to include as GM.")]
            public async Task AddGMRole(IRole role)
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (guild.AddGMRole(role))
                {
                    await Context.Interaction.RespondAsync($"{role.Name} was added to GM Roles!");
                }
                else
                {
                    await Context.Interaction.RespondAsync(
                        $"{role.Name} **could not** be added to GM Roles!"
                    );
                }
            }

            [SlashCommand("remove", "Remove a role to include as GM.")]
            public async Task RemoveGMRole(IRole role)
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (guild.RemoveGMRole(role))
                {
                    await Context.Interaction.RespondAsync(
                        $"{role.Name} was removed from GM Roles!"
                    );
                }
                else
                {
                    await Context.Interaction.RespondAsync(
                        $"{role.Name} **could not** be removed from GM Roles!"
                    );
                }
            }

            [SlashCommand("list", "List current roles associated with GMs.")]
            public async Task ListGMRoles()
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                string roles = string.Join("\n", guild.GMRoles);
                if (roles == null || roles == string.Empty)
                {
                    roles = "_n/a_";
                }
                await Context.Interaction.RespondAsync(
                    embed: new EmbedBuilder().WithDescription(roles).WithTitle("GM Roles").Build()
                );
            }
        }

        [DefaultMemberPermissions(GuildPermission.Administrator)]
        [Group("board", "Perform management of the forum boards which the bot posts in.")]
        public class BoardManagement : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("quest", "Assign the forum board for quest posts by the bot.")]
            public async Task AssignQuestBoard(IForumChannel forumChannel)
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync(
                        "Only GMs may run this command!",
                        ephemeral: true
                    );
                    return;
                }
                guild.QuestBoard = forumChannel;
                guild.QueueSave("boards");

                await Context.Interaction.RespondAsync(
                    "Quest board has been assigned to " + forumChannel.Mention
                );
            }

            [SlashCommand("character", "Assign the forum board for character posts by the bot.")]
            public async Task AssignCharacterBoard(IForumChannel forumChannel)
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync(
                        "Only GMs may run this command!",
                        ephemeral: true
                    );
                    return;
                }
                guild.CharacterBoard = forumChannel;
                guild.QueueSave("boards");

                await Context.Interaction.RespondAsync(
                    "Character board has been assigned to " + forumChannel.Mention
                );
            }

            [SlashCommand(
                "transaction",
                "Assign the forum board for transaction posts by the bot."
            )]
            public async Task AssignTransactionBoard(IForumChannel forumChannel)
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync(
                        "Only GMs may run this command!",
                        ephemeral: true
                    );
                    return;
                }
                guild.TransactionBoard = forumChannel;
                guild.SaveAll();

                await Context.Interaction.RespondAsync(
                    "Transaction board has been assigned to " + forumChannel.Mention
                );
            }
        }
    }
}
