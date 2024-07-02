using Bot.Guilds;
using Bot.Quests;
using Discord;
using Discord.WebSocket;

namespace Bot
{
    public partial class BotManager
    {
        private static async Task QuestCreateConfirm(SocketMessageComponent button)
        {
            IUser user = button.User;
            string gm = button.Data.CustomId.Split("+")[2];
            if (user.Username == gm)
            {
                string guildId = button.Data.CustomId.Split("+")[1];
                Guild guild = GetGuild(ulong.Parse(guildId));

                string name = button.Data.CustomId.Split("+")[3];

                Quest? quest = guild.GetQuest(name, user);
                if (quest != null)
                {
                    await PostQuest(button, guild, quest, user);
                }
                else
                {
                    await button.UpdateAsync(x =>
                    {
                        x.Content = "Somehow Quest was null.";
                    });
                }
            }
            else
            {
                await button.UpdateAsync(x => { });
            }
        }

        private static async Task QuestCreateBack(SocketMessageComponent button)
        {
            IUser user = button.User;
            string gm = button.Data.CustomId.Split("+")[2];
            if (user.Username == gm)
            {
                string guildId = button.Data.CustomId.Split("+")[1];
                Guild guild = GetGuild(ulong.Parse(guildId));

                string name = button.Data.CustomId.Split("+")[3];

                Quest? quest = guild.GetQuest(name, user);
                if (quest != null)
                {
                    var threatMenu = Quest.ThreatSelector(guildId, gm, name);
                    var messageComponents = new ComponentBuilder()
                        .WithSelectMenu(threatMenu)
                        .WithButton(
                            "Back",
                            "createQuest+" + guildId + "+" + gm + "+" + name + "+back",
                            ButtonStyle.Danger
                        );
                    var questEmbed = quest.GenerateEmbed(user);
                    await button.UpdateAsync(x =>
                    {
                        x.Embed = questEmbed.Build();
                        x.Components = messageComponents.Build();
                    });
                }
                else
                {
                    await button.UpdateAsync(x =>
                    {
                        x.Content = "Somehow Quest was null.";
                    });
                }
            }
            else
            {
                await button.UpdateAsync(x => { });
            }
        }

        private static async Task QuestCreateCancel(SocketMessageComponent button)
        {
            IUser user = button.User;
            string gm = button.Data.CustomId.Split("+")[2];
            if (user.Username == gm)
            {
                string guildId = button.Data.CustomId.Split("+")[1];
                Guild guild = GetGuild(ulong.Parse(guildId));

                string name = button.Data.CustomId.Split("+")[3];

                Quest? quest = guild.GetQuest(name, user);
                if (quest != null)
                {
                    guild.RemoveQuest(quest);
                    await button.UpdateAsync(x =>
                    {
                        x.Content = "Quest creation canceled.";
                        x.Embed = null;
                        x.Components = null;
                    });
                }
                else
                {
                    await button.UpdateAsync(x =>
                    {
                        x.Content = "Somehow Quest was null.";
                    });
                }
            }
            else
            {
                await button.UpdateAsync(x => { });
            }
        }
    }
}
