using OpenRoadHelper.Guilds;
using Discord;
using Discord.Interactions;

namespace OpenRoadHelper.Quests
{
    [RequireGM(Group = "availability")]
    [Group("availability", "Update your availability for GMing.")]
    public class AvailabilityManagement : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("Add", "Add a new availability slot for GMing.")]
        public async Task AddAvailability(uint earlyStart, uint latestStart, uint cutoff)
        {
            IUser gm = Context.User;
            ulong guildId = Context.Guild.Id;

            if (!Manager.IsGamemaster(gm, guildId))
            {
                await Context.Interaction.RespondAsync(
                    "Only GMs can use this command.",
                    ephemeral: true
                );
                return;
            }

            var embed = new EmbedBuilder()
                .AddField("Early Start", "<t:" + earlyStart + ":F>", true)
                .AddField("Late Start", "<t:" + latestStart + ":F>", true)
                .AddField("Cutoff", "<t:" + cutoff + ":F>", true);

            await Context.Interaction.RespondAsync(embed: embed.Build(), ephemeral: true);

            Timeframe newTimeframe =
                new()
                {
                    EarliestStart = Generic.DateTimeFromUnixSeconds(earlyStart),
                    LatestStart = Generic.DateTimeFromUnixSeconds(latestStart),
                    Cutoff = Generic.DateTimeFromUnixSeconds(cutoff)
                };

            Guild guild = Manager.GetGuild(Context.Guild.Id);
            guild.AddAvailability(gm, newTimeframe);
        }

        [SlashCommand("Replace", "Replace an existing availability slot for GMing.")]
        public async Task ReplaceAvailability(
            uint earlyStart = 0,
            uint latestStart = 0,
            uint cutoff = 0
        )
        {
            IUser gm = Context.User;
            ulong guildId = Context.Guild.Id;

            if (!Manager.IsGamemaster(gm, guildId))
            {
                await Context.Interaction.RespondAsync(
                    "Only GMs can use this command.",
                    ephemeral: true
                );
                return;
            }

            var embed = new EmbedBuilder()
                .AddField("Early Start", "<t:" + earlyStart + ":F>", true)
                .AddField("Late Start", "<t:" + latestStart + ":F>", true)
                .AddField("Cutoff", "<t:" + cutoff + ":F>", true);

            await Context.Interaction.RespondAsync(embed: embed.Build(), ephemeral: true);

            Timeframe newTimeframe =
                new()
                {
                    EarliestStart = Generic.DateTimeFromUnixSeconds(earlyStart),
                    LatestStart = Generic.DateTimeFromUnixSeconds(latestStart),
                    Cutoff = Generic.DateTimeFromUnixSeconds(cutoff)
                };

            Guild guild = Manager.GetGuild(Context.Guild.Id);
            guild.ReplaceAvailability(gm, newTimeframe);
        }
    }
}
