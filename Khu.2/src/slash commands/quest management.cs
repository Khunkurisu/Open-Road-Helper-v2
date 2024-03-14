using Discord;
using Discord.Interactions;

namespace Bot.Quests
{
    [RequireRole("Game Master", Group = "quest")]
    [RequireRole("Dungeon Master", Group = "quest")]
    [RequireRole("GM", Group = "quest")]
    [RequireRole("DM", Group = "quest")]
    [Group("quest", "Manage quests with these commands.")]
    public class QuestManagement : InteractionModuleBase<SocketInteractionContext>
    {
        public static readonly Dictionary<string, GMAvailability> GMAvailabilities = new();

        [SlashCommand("create", "Create a quest.")]
        public async Task CreateQuest(
            string name,
            string desc,
            int threat,
            string tags,
            int maxPlayers
        )
        {
            await RespondAsync(name);
            IUser gm = Context.User;
        }

        [SlashCommand("availability", "Update your availability for GMing.")]
        public async Task ModifyAvailability(uint earlyStart, uint latestStart, uint cutoff)
        {
            await RespondAsync("<t:" + earlyStart + ":F>");
            await RespondAsync("<t:" + latestStart + ":F>");
            await RespondAsync("<t:" + cutoff + ":F>");
            IUser gm = Context.User;
            Timeframe newTimeframe =
                new()
                {
                    EarliestStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(
                        earlyStart
                    ),
                    LatestStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(
                        latestStart
                    ),
                    Cutoff = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(cutoff),
                };

            if (GMAvailabilities.ContainsKey(gm.Username))
            {
                GMAvailability gmAvailability = GMAvailabilities[gm.Username];
                gmAvailability.AddTimeframe(newTimeframe);
            }
        }
    }
}
