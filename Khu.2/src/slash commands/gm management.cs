using Bot.Guilds;
using Bot.Helpers;
using Discord;
using Discord.Interactions;

namespace Bot.GameMaster
{
    [RequireRole("Game Master", Group = "quest")]
    [RequireRole("Dungeon Master", Group = "quest")]
    [RequireRole("GM", Group = "quest")]
    [RequireRole("DM", Group = "quest")]
    [Group("gm", "Perform gm related tasks with these commands.")]
    public class GameMasterManagement : InteractionModuleBase<SocketInteractionContext>
    {
        [Group("refresh", "Refresh posts for the given list.")]
        public class RefreshListing : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("characters", "Refresh all character posts with database values.")]
            public async Task RefreshCharacterPosts()
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
				if(!guild.IsGamemaster(Context.User)) {
					await Context.Interaction.RespondAsync("Only GMs may run this command!");
					return;
				}
                await guild.RefreshCharacterPosts(Context);
                await Context.Interaction.RespondAsync("Character posts refreshed!");
            }

            [SlashCommand("quests", "Refresh all character posts with database values.")]
            public async Task RefreshQuestPosts()
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
				if(!guild.IsGamemaster(Context.User)) {
					await Context.Interaction.RespondAsync("Only GMs may run this command!");
					return;
				}
                await guild.RefreshQuestPosts();
                await Context.Interaction.RespondAsync("Character posts refreshed!");
            }
        }

        [Group("gm-roles", "Manage roles assigned as Game Master.")]
        public class GameMasterRoles : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("add", "Add a role to include as GM.")]
            public async Task AddGMRole(IRole role)
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
				if(!guild.IsGamemaster(Context.User)) {
					await Context.Interaction.RespondAsync("Only GMs may run this command!");
					return;
				}
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
				if(!guild.IsGamemaster(Context.User)) {
					await Context.Interaction.RespondAsync("Only GMs may run this command!");
					return;
				}
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
				if(!guild.IsGamemaster(Context.User)) {
					await Context.Interaction.RespondAsync("Only GMs may run this command!");
					return;
				}
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

        [Group("board", "Perform management of the forum boards which the bot posts in.")]
        public class BoardManagement : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("quest", "Assign the forum board for quest posts by the bot.")]
            public async Task AssignQuestBoard(IForumChannel forumChannel)
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
				if(!guild.IsGamemaster(Context.User)) {
					await Context.Interaction.RespondAsync("Only GMs may run this command!");
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
				if(!guild.IsGamemaster(Context.User)) {
					await Context.Interaction.RespondAsync("Only GMs may run this command!");
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
				if(!guild.IsGamemaster(Context.User)) {
					await Context.Interaction.RespondAsync("Only GMs may run this command!");
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
