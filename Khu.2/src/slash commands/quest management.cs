using Bot.Guilds;
using Bot.Helpers;
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
                    EarliestStart = GenericHelpers.DateTimeFromUnixSeconds(earlyStart),
                    LatestStart = GenericHelpers.DateTimeFromUnixSeconds(latestStart),
                    Cutoff = GenericHelpers.DateTimeFromUnixSeconds(cutoff)
                };

            Guild guild = BotManager.GetGuildById(Context.Guild.Id);
            if (guild.GMAvailabilities.ContainsKey(gm.Username))
            {
                GMAvailability gmAvailability = guild.GMAvailabilities[gm.Username];
                gmAvailability.AddTimeframe(newTimeframe);
            }
            else
            {
                GMAvailability gmAvailability = new(gm, newTimeframe);
                guild.GMAvailabilities.Add(gm.Username, gmAvailability);
            }
        }
    }
}
