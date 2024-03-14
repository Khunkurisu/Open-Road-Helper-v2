using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Bot.Quests
{
    [RequireRole("Game Master", Group = "quest")]
    [RequireRole("Dungeon Master", Group = "quest")]
    [RequireRole("GM", Group = "quest")]
    [RequireRole("DM", Group = "quest")]
    [Group("quest", "Manage quests with these commands.")]
    public class QuestManagement : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("create", "Create a quest.")]
        public async Task CreateQuest(
            string name,
            string desc,
            int threat,
            string tags,
            long time,
            int level,
            int maxPlayers,
            ulong messageId,
            ulong threadId
        )
        {
            await RespondAsync(name);
            SocketUser gm = Context.Client.CurrentUser;
        }
    }
}
