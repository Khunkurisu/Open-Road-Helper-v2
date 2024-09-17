using OpenRoadHelper.Guilds;
using OpenRoadHelper.Quests;
using Discord;
using Discord.WebSocket;

namespace OpenRoadHelper
{
    public partial class Manager
    {
        private static async Task QuestCreateConfirmOLD(SocketMessageComponent button)
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

        private static async Task QuestCreateBack(SocketMessageComponent component)
        {
            ulong? guildIdOrNull = component.GuildId;
            if (guildIdOrNull == null)
            {
				await InteractionErrors.MustRunInGuild(component, component.User);
                return;
            }
            ulong guildId = (ulong)guildIdOrNull;
            Guild guild = GetGuild(guildId);
            FormValue formValues = guild.GetFormValues(component.Data.CustomId);
            ulong gmId = formValues.User;
            IUser? gamemaster = GetGuildUser(guildId, gmId);
            IUser user = component.User;
            if (gamemaster != null && user.Id == gmId)
            {
                string name = formValues.Target;

                Quest? quest = guild.GetQuest(name, user);
                if (quest != null)
                {
                    var threatMenu = Quest.ThreatSelector(guildId, gmId, name);
                    var messageComponents = new ComponentBuilder()
                        .WithSelectMenu(threatMenu)
                        .WithButton(
                            "Back",
                            guild.GenerateFormValues(
                                new($"createQuest", gmId, name, "back", new())
                            ),
                            ButtonStyle.Danger
                        );
                    var questEmbed = quest.GenerateEmbed(user);
                    await component.UpdateAsync(x =>
                    {
                        x.Embed = questEmbed.Build();
                        x.Components = messageComponents.Build();
                    });
                }
                else
                {
                    await component.UpdateAsync(x =>
                    {
                        x.Content = "Somehow Quest was null.";
                    });
                }
            }
            else
            {
                await component.UpdateAsync(x => { });
            }
        }

        private static async Task QuestCreateCancelOLD(SocketMessageComponent button)
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
