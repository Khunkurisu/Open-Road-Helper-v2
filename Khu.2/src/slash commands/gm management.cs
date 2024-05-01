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
        [Group("board", "Perform management of the forum boards which the bot posts in.")]
        public class BoardManagement : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("quest", "Assign the forum board for quest posts by the bot.")]
            public async Task AssignQuestBoard(IForumChannel forumChannel)
            {
                Guild guild = BotManager.GetGuild(Context.Guild.Id);
                guild.QuestBoard = forumChannel;
                guild.SaveAll();

                await Context.Interaction.RespondAsync(
                    "Quest board has been assigned to " + forumChannel.Mention
                );
            }

            [SlashCommand("character", "Assign the forum board for character posts by the bot.")]
            public async Task AssignCharacterBoard(IForumChannel forumChannel)
            {
                Guild guild = BotManager.GetGuild(Context.Guild.Id);
                guild.CharacterBoard = forumChannel;
                guild.SaveAll();

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
                Guild guild = BotManager.GetGuild(Context.Guild.Id);
                guild.TransactionBoard = forumChannel;
                guild.SaveAll();

                await Context.Interaction.RespondAsync(
                    "Transaction board has been assigned to " + forumChannel.Mention
                );
            }

            [SlashCommand("list", "List the quests currently stored.")]
            public async Task ListQuests()
            {
                Guild guild = BotManager.GetGuild(Context.Guild.Id);
                string message = "";
                if (guild.QuestBoard != null)
                {
                    message += "## Quest Board\n" + guild.QuestBoard.Mention + "\n";
                }
                if (guild.CharacterBoard != null)
                {
                    message += "## Character Board\n" + guild.CharacterBoard.Mention + "\n";
                }
                if (guild.TransactionBoard != null)
                {
                    message += "## Transaction Board\n" + guild.TransactionBoard.Mention + "\n";
                }
                if (message == "")
                {
                    message = "No boards have been assigned.";
                }

                await Context.Interaction.RespondAsync(message.TrimEnd());
            }
        }
    }
}
