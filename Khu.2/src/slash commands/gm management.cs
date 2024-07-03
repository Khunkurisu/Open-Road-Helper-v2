using Bot.Characters;
using Bot.Guilds;
using Discord;
using Discord.Interactions;

namespace Bot.GameMaster
{
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
                if (!guild.IsGamemaster(Context.User))
                {
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
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync("Only GMs may run this command!");
                    return;
                }
                await guild.RefreshQuestPosts();
                await Context.Interaction.RespondAsync("Character posts refreshed!");
            }
        }

        [Group("currency", "Manage player currency values")]
        public class CurrencyManagement : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("add-pt", "Increase player PT by an amount.")]
            public async Task AddPT(IUser user, uint amount)
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync("Only GMs may run this command!");
                    return;
                }
                uint oldPT = guild.GetPlayerTokenCount(user.Id);
                guild.IncreasePlayerTokenCount(user.Id, amount);
                await Context.Interaction.RespondAsync(
                    $"PT for {user.Username} updated from {oldPT} PT to {guild.GetPlayerTokenCount(user.Id)} PT."
                );
                await guild.RefreshCharacterPosts(user.Id);
            }

            [SlashCommand("sub-pt", "Decrease player PT by an amount.")]
            public async Task SubtractPT(IUser user, uint amount)
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync("Only GMs may run this command!");
                    return;
                }
                uint oldPT = guild.GetPlayerTokenCount(user.Id);
                guild.DecreasePlayerTokenCount(user.Id, amount);
                await Context.Interaction.RespondAsync(
                    $"PT for {user.Username} updated from {oldPT} PT to {guild.GetPlayerTokenCount(user.Id)} PT."
                );
                await guild.RefreshCharacterPosts(user.Id);
            }

            [SlashCommand("set-pt", "Set player PT to a specific amount.")]
            public async Task SetPT(IUser user, uint amount)
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync("Only GMs may run this command!");
                    return;
                }
                uint oldPT = guild.GetPlayerTokenCount(user.Id);
                guild.SetPlayerTokenCount(user.Id, amount);
                await Context.Interaction.RespondAsync(
                    $"PT for {user.Username} updated from {oldPT} PT to {guild.GetPlayerTokenCount(user.Id)} PT."
                );
                await guild.RefreshCharacterPosts(user.Id);
            }

            [SlashCommand("add-gp", "Increase character GP by an amount.")]
            public async Task AddGP(
                IUser user,
                [Autocomplete(typeof(CharacterNameAutocompleteHandler))] string charName,
                float amount
            )
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync("Only GMs may run this command!");
                    return;
                }
                Character? character = guild.GetCharacter(user.Id, charName);
                if (character == null)
                {
                    await Context.Interaction.RespondAsync(
                        $"Unable to find character '{charName}' for user <@{user.Id}>"
                    );
                    return;
                }
                double oldGP = character.Gold;
                character.Gold += amount;
                await Context.Interaction.RespondAsync(
                    $"Gold for {charName} updated from {oldGP} gp to {character.Gold} gp."
                );
                await Manager.DrawCharacterPost(character);
            }

            [SlashCommand("sub-gp", "Decrease character GP by an amount.")]
            public async Task SubtractGP(
                IUser user,
                [Autocomplete(typeof(CharacterNameAutocompleteHandler))] string charName,
                float amount
            )
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync("Only GMs may run this command!");
                    return;
                }
                Character? character = guild.GetCharacter(user.Id, charName);
                if (character == null)
                {
                    await Context.Interaction.RespondAsync(
                        $"Unable to find character '{charName}' for user <@{user.Id}>"
                    );
                    return;
                }
                double oldGP = character.Gold;
                character.Gold -= amount;
                await Context.Interaction.RespondAsync(
                    $"Gold for {charName} updated from {oldGP} gp to {character.Gold} gp."
                );
                await Manager.DrawCharacterPost(character);
            }

            [SlashCommand("set-gp", "Set character GP to a specific amount.")]
            public async Task SetGP(
                IUser user,
                [Autocomplete(typeof(CharacterNameAutocompleteHandler))] string charName,
                float amount
            )
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync("Only GMs may run this command!");
                    return;
                }
                Character? character = guild.GetCharacter(user.Id, charName);
                if (character == null)
                {
                    await Context.Interaction.RespondAsync(
                        $"Unable to find character '{charName}' for user <@{user.Id}>"
                    );
                    return;
                }
                double oldGP = character.Gold;
                character.Gold = amount;
                await Context.Interaction.RespondAsync(
                    $"Gold for {charName} updated from {oldGP} gp to {character.Gold} gp."
                );
                await Manager.DrawCharacterPost(character);
            }

            [SlashCommand("add-dt", "Increase character DT by an amount.")]
            public async Task AddDT(
                IUser user,
                [Autocomplete(typeof(CharacterNameAutocompleteHandler))] string charName,
                uint amount
            )
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync("Only GMs may run this command!");
                    return;
                }
                Character? character = guild.GetCharacter(user.Id, charName);
                if (character == null)
                {
                    await Context.Interaction.RespondAsync(
                        $"Unable to find character '{charName}' for user <@{user.Id}>"
                    );
                    return;
                }
                uint oldDT = character.Downtime;
                character.Downtime += amount;
                await Context.Interaction.RespondAsync(
                    $"Downtime for {charName} updated from {oldDT} DT to {character.Downtime} DT."
                );
                await Manager.DrawCharacterPost(character);
            }

            [SlashCommand("sub-dt", "Decrease character DT by an amount.")]
            public async Task SubtractDT(
                IUser user,
                [Autocomplete(typeof(CharacterNameAutocompleteHandler))] string charName,
                uint amount
            )
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync("Only GMs may run this command!");
                    return;
                }
                Character? character = guild.GetCharacter(user.Id, charName);
                if (character == null)
                {
                    await Context.Interaction.RespondAsync(
                        $"Unable to find character '{charName}' for user <@{user.Id}>"
                    );
                    return;
                }
                uint oldDT = character.Downtime;
                character.Downtime -= amount;
                await Context.Interaction.RespondAsync(
                    $"Downtime for {charName} updated from {oldDT} DT to {character.Downtime} DT."
                );
                await Manager.DrawCharacterPost(character);
            }

            [SlashCommand("set-dt", "Set character DT to a specific amount.")]
            public async Task SetDT(
                IUser user,
                [Autocomplete(typeof(CharacterNameAutocompleteHandler))] string charName,
                uint amount
            )
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync("Only GMs may run this command!");
                    return;
                }
                Character? character = guild.GetCharacter(user.Id, charName);
                if (character == null)
                {
                    await Context.Interaction.RespondAsync(
                        $"Unable to find character '{charName}' for user <@{user.Id}>"
                    );
                    return;
                }
                uint oldDT = character.Downtime;
                character.Downtime = amount;
                await Context.Interaction.RespondAsync(
                    $"Downtime for {charName} updated from {oldDT} DT to {character.Downtime} DT."
                );
                await Manager.DrawCharacterPost(character);
            }
        }

        [Group("gm-roles", "Manage roles assigned as Game Master.")]
        public class GameMasterRoles : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("add", "Add a role to include as GM.")]
            public async Task AddGMRole(IRole role)
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
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
                if (!guild.IsGamemaster(Context.User))
                {
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
                if (!guild.IsGamemaster(Context.User))
                {
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
                if (!guild.IsGamemaster(Context.User))
                {
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
                if (!guild.IsGamemaster(Context.User))
                {
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
                if (!guild.IsGamemaster(Context.User))
                {
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
