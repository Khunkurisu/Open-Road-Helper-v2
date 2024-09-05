using System.Windows.Markup;
using OpenRoadHelper.Guilds;
using OpenRoadHelper.Quests;
using Discord;
using Discord.WebSocket;

namespace OpenRoadHelper
{
    public partial class Manager
    {
        private static async Task QuestCreate(SocketMessageComponent selectMenu)
        {
            ulong? guildIdOrNull = selectMenu.GuildId;
            if (guildIdOrNull == null)
            {
                await selectMenu.RespondAsync("This command can only be used in a guild.");
                return;
            }
            ulong guildId = (ulong)guildIdOrNull;
            Guild guild = GetGuild(guildId);
            FormValue formValues = guild.GetFormValues(selectMenu.Data.CustomId);
            ulong gmId = formValues.User;
            IUser? gamemaster = GetGuildUser(guildId, gmId);
            IUser user = selectMenu.User;
            if (gamemaster != null && user.Id == gmId)
            {
                string name = formValues.Target;
                string menuType = formValues.Modifier;
                string value = string.Join(", ", selectMenu.Data.Values);
                var maxPlayerMenu = Quest.PlayerMaxSelector(guildId, gmId, name);
                var messageComponents = new ComponentBuilder();

                Quest? quest = guild.GetQuest(name, user);
                if (quest != null)
                {
                    switch (menuType)
                    {
                        case "questPlayerMax":
                        {
                            quest.SetMaxPlayers(value);
                            messageComponents = Quest.ConfirmationButtons(guildId, gmId, name, "questCreate");
                            break;
                        }
                        case "questThreat":
                        {
                            quest.SetThreat(value);
                            messageComponents
                                .WithSelectMenu(maxPlayerMenu)
                                .WithButton(
                                    "Back",
                                    guild.GenerateFormValues(
                                        new($"createQuest", gmId, name, "back", new())
                                    ),
                                    ButtonStyle.Danger
                                );
                            break;
                        }
                    }

                    guild.SaveAll();

                    var questEmbed = quest.GenerateEmbed(user);

                    await selectMenu.UpdateAsync(x =>
                    {
                        x.Content = "Always double-check things are correct before continuing!";
                        x.Embed = questEmbed.Build();
                        x.Components = messageComponents.Build();
                    });
                }
                else
                {
                    await selectMenu.UpdateAsync(x =>
                    {
                        x.Content = "Somehow Quest was null.";
                    });
                }
            }
            else
            {
                await selectMenu.UpdateAsync(x =>
                {
                    x.Content = "User id doesn't match.";
                });
            }
        }

        private static async Task QuestCreate(
            SocketModal modal,
            List<SocketMessageComponentData> components
        )
        {
            ulong? guildIdOrNull = modal.GuildId;
            if (guildIdOrNull == null)
            {
                await modal.RespondAsync("This command can only be used in a guild.");
                return;
            }
            ulong guildId = (ulong)guildIdOrNull;
            Guild guild = GetGuild(guildId);
            FormValue formValues = guild.GetFormValues(modal.Data.CustomId);
            ulong gmId = formValues.User;
            IUser gamemaster = modal.User;

            string name = components.First(x => x.CustomId == "quest_name").Value;
            string description = components.First(x => x.CustomId == "quest_description").Value;

            List<string> imgUrls = formValues.Metadata;
            string filepath = @".\data\images\quests\";
            Directory.CreateDirectory(filepath);
            foreach (string url in imgUrls)
            {
                string filename = $"{name}--initial.webp";

                if (!await Generic.DownloadImage(url, filepath + filename))
                {
                    await modal.RespondAsync("Failed to process image.", ephemeral: true);
                    return;
                }
            }

            Quest quest = new(gamemaster.Id, guildId, name, description, new());
            guild.AddQuest(quest);

            var messageComponents = new ComponentBuilder();
            var threatMenu = Quest.ThreatSelector(guildId, gmId, name);
            var maxPlayerMenu = Quest.PlayerMaxSelector(guildId, gmId, name);
            messageComponents
                .WithSelectMenu(threatMenu)
                .WithButton(
                    "Cancel",
                    guild.GenerateFormValues(new($"createQuest", gmId, name, "cancel", new())),
                    ButtonStyle.Danger
                );

            var questEmbed = quest.GenerateEmbed(gamemaster);

            await modal.RespondAsync(
                embed: questEmbed.Build(),
                components: messageComponents.Build()
            );
        }

        private static async Task PostQuestOLD(
            SocketMessageComponent context,
            Guild guild,
            Quest quest,
            IUser gm
        )
        {
            guild.SaveAll();
            if (guild.QuestBoard != null)
            {
                EmbedBuilder questEmbed = quest.GenerateEmbed(gm);
                ForumTag[] tags = { guild.QuestBoard.Tags.First(x => x.Name == "Pending") };
                ComponentBuilder components = new ComponentBuilder().WithButton(
                    "Edit",
                    $"editQuest+{guild.Id}+{quest.Name}+editStart",
                    ButtonStyle.Secondary
                );
                IThreadChannel thread = await guild.QuestBoard.CreatePostAsync(
                    quest.Name,
                    ThreadArchiveDuration.OneWeek,
                    embed: questEmbed.Build(),
                    tags: tags,
                    components: components.Build()
                );
                await thread.AddUserAsync((IGuildUser)gm);
                await context.UpdateAsync(x =>
                {
                    x.Content = "Quest posted! " + thread.Mention;
                    x.Embed = null;
                    x.Components = null;
                });
            }
        }
    }
}
