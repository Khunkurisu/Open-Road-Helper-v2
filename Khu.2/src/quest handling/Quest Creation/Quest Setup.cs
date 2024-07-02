using Bot.Guilds;
using Bot.Quests;
using Discord;
using Discord.WebSocket;

namespace Bot
{
    public partial class BotManager
    {
        private static async Task QuestCreate(SocketMessageComponent selectMenu)
        {
            string gm = selectMenu.Data.CustomId.Split("+")[2];
            IUser user = selectMenu.User;
            if (user.Username == gm)
            {
                string guildId = selectMenu.Data.CustomId.Split("+")[1];
                Guild guild = GetGuild(ulong.Parse(guildId));

                string name = selectMenu.Data.CustomId.Split("+")[3];
                string menuType = selectMenu.Data.CustomId.Split("+")[4];
                string value = string.Join(", ", selectMenu.Data.Values);
                var maxPlayerMenu = Quest.PlayerMaxSelector(guildId, gm, name);
                var messageComponents = new ComponentBuilder();

                Quest? quest = guild.GetQuest(name, user);
                if (quest != null)
                {
                    switch (menuType)
                    {
                        case "questPlayerMax":
                        {
                            quest.SetMaxPlayers(value);
                            messageComponents = Quest.ConfirmationButtons(guildId, gm, name);
                            break;
                        }
                        case "questThreat":
                        {
                            quest.SetThreat(value);
                            messageComponents
                                .WithSelectMenu(maxPlayerMenu)
                                .WithButton(
                                    "Back",
                                    "createQuest+" + guildId + "+" + gm + "+" + name + "+back",
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
            string gm = modal.Data.CustomId.Split("+")[2];
            IUser gamemaster = modal.User;
            string guildId = modal.Data.CustomId.Split("+")[1];
            Guild guild = GetGuild(ulong.Parse(guildId));

            string name = components.First(x => x.CustomId == "quest_name").Value;
            string description = components.First(x => x.CustomId == "quest_description").Value;
            string allTags = components.First(x => x.CustomId == "quest_tags").Value;
            string[] tags = allTags.Split(
                ",",
                StringSplitOptions.TrimEntries & StringSplitOptions.RemoveEmptyEntries
            );
            Quest quest = new(gamemaster.Id, name, description, new(tags));
            guild.AddQuest(quest);

            var messageComponents = new ComponentBuilder(); //Quest.ConfirmationButtons(guildId, gm, name);
            var threatMenu = Quest.ThreatSelector(guildId, gm, name);
            var maxPlayerMenu = Quest.PlayerMaxSelector(guildId, gm, name);
            messageComponents
                .WithSelectMenu(threatMenu)
                .WithButton(
                    "Cancel",
                    "createQuest+" + guildId + "+" + gm + "+" + name + "+cancel",
                    ButtonStyle.Danger
                );

            var questEmbed = quest.GenerateEmbed(gamemaster);

            await modal.RespondAsync(
                embed: questEmbed.Build(),
                components: messageComponents.Build()
            );
        }

        private static async Task PostQuest(
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
                    "editQuest+" + guild.Id + "+" + quest.Name + "+editStart",
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
