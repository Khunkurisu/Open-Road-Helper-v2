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
        public async Task CreateQuest()
        {
            ulong guildId = Context.Guild.Id;
            IUser gm = Context.User;

            if (BotManager.IsGamemaster(gm, guildId))
            {
                var textBox = new ModalBuilder()
                    .WithTitle("Create Quest")
                    .WithCustomId("createQuest+" + guildId + "+" + gm.Username)
                    .AddTextInput("Name", "quest_name", placeholder: "Quest Name")
                    .AddTextInput(
                        "Description",
                        "quest_description",
                        TextInputStyle.Paragraph,
                        "Quest description."
                    )
                    .AddTextInput("Tags", "quest_tags", placeholder: "Comma, Separated, Tags");

                await Context.Interaction.RespondWithModalAsync(textBox.Build());
            }
            else
            {
                await Context.Interaction.RespondAsync(
                    "Only GMs can use this command.",
                    ephemeral: true
                );
            }
        }

        [SlashCommand("list", "List the quests currently stored.")]
        public async Task ListQuests()
        {
            ulong guildId = Context.Guild.Id;
            IUser gm = Context.User;

            if (BotManager.IsGamemaster(gm, guildId))
            {
                Guild guild = BotManager.GetGuild(guildId);

                List<Embed> questEmbeds = new();
                int count = 0;
                foreach (Quest quest in guild.Quests)
                {
                    questEmbeds.Add(quest.GenerateEmbed(gm).Build());
                    count++;
                    if (count >= 10)
                    {
                        break;
                    }
                }

                await Context.Interaction.RespondAsync(embeds: questEmbeds.ToArray());
            }
            else
            {
                await Context.Interaction.RespondAsync(
                    "Only GMs can use this command.",
                    ephemeral: true
                );
            }
        }

        [SlashCommand("availability", "Update your availability for GMing.")]
        public async Task ModifyAvailability(uint earlyStart, uint latestStart, uint cutoff)
        {
            IUser gm = Context.User;
            ulong guildId = Context.Guild.Id;

            if (BotManager.IsGamemaster(gm, guildId))
            {
                await RespondAsync("<t:" + earlyStart + ":F>");
                await RespondAsync("<t:" + latestStart + ":F>");
                await RespondAsync("<t:" + cutoff + ":F>");

                Timeframe newTimeframe =
                    new()
                    {
                        EarliestStart = GenericHelpers.DateTimeFromUnixSeconds(earlyStart),
                        LatestStart = GenericHelpers.DateTimeFromUnixSeconds(latestStart),
                        Cutoff = GenericHelpers.DateTimeFromUnixSeconds(cutoff)
                    };

                Guild guild = BotManager.GetGuild(Context.Guild.Id);
                guild.AddAvailability(gm, newTimeframe);
            }
            else
            {
                await Context.Interaction.RespondAsync(
                    "Only GMs can use this command.",
                    ephemeral: true
                );
            }
        }
    }
}
