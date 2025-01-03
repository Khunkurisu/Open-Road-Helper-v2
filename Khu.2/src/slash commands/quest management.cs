using OpenRoadHelper.Guilds;
using Discord;
using Discord.Interactions;

namespace OpenRoadHelper.Quests
{
    [RequireGM(Group = "quest")]
    [Group("quest", "Manage quests with these commands.")]
    public class QuestManagement : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("create", "Create a quest.")]
        public async Task CreateQuest(
            [Summary("image", "Flavor image or splash art for quest.")] IAttachment? image = null
        )
        {
            ulong guildId = Context.Guild.Id;
            IUser gm = Context.User;

            if (!Manager.IsGamemaster(gm, guildId))
            {
                await Context.Interaction.RespondAsync(
                    "Only GMs can use this command.",
                    ephemeral: true
                );
                return;
            }

            List<string> url = new();

            if (image != null)
            {
                url.Add(image.Url);
            }

            Guild guild = Manager.GetGuild(guildId);
            var textBox = new ModalBuilder()
                .WithTitle("Create Quest")
                .WithCustomId(
                    guild.GenerateFormValues(
                        new("createQuest", gm.Id, string.Empty, string.Empty, url)
                    )
                )
                .AddTextInput(
                    "Name",
                    "quest_name",
                    TextInputStyle.Short,
                    "Quest Name",
                    5,
                    required: true
                )
                .AddTextInput(
                    "Description",
                    "quest_description",
                    TextInputStyle.Paragraph,
                    "Detailed description of the quest.",
                    10,
                    required: true
                );

            await Context.Interaction.RespondWithModalAsync(textBox.Build());
        }

        [SlashCommand("update", "Updates a given quest.")]
        public async Task UpdateQuest([Summary("name", "Name of the quest.")] string name)
        {
            ulong guildId = Context.Guild.Id;
            IUser gm = Context.User;

            if (!Manager.IsGamemaster(gm, guildId))
            {
                await Context.Interaction.RespondAsync(
                    "Only GMs can use this command.",
                    ephemeral: true
                );
                return;
            }

            Guild guild = Manager.GetGuild(guildId);

            Quest? quest = guild.GetQuest(name, gm);
            if (quest == null)
            {
                await Context.Interaction.RespondAsync(
                    "Unable to access quest data.",
                    ephemeral: true
                );
                return;
            }
        }

        [SlashCommand("list", "List the quests currently stored.")]
        public async Task ListQuests()
        {
            ulong guildId = Context.Guild.Id;
            IUser gm = Context.User;

            if (!Manager.IsGamemaster(gm, guildId))
            {
                await Context.Interaction.RespondAsync(
                    "Only GMs can use this command.",
                    ephemeral: true
                );
                return;
            }

            Guild guild = Manager.GetGuild(guildId);

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
    }
}
