using OpenRoadHelper.Guilds;
using Discord;
using Discord.WebSocket;

namespace OpenRoadHelper.Quests
{
    public partial class Quest
    {
        public static SelectMenuBuilder ThreatSelector(ulong guildId, ulong gm, string name)
        {
            Guild guild = Manager.GetGuild(guildId);
            return new SelectMenuBuilder()
                .WithPlaceholder("Quest Threat")
                .WithCustomId(
                    guild.GenerateFormValues(new($"createQuest", gm, name, "questThreat", new()))
                )
                .WithMinValues(1)
                .AddOption("Trivial", "Trivial", "Highest threat encounter is trivial.")
                .AddOption("Low", "Low", "Highest threat encounter is low.")
                .AddOption("Moderate", "Moderate", "Highest threat encounter is moderate.")
                .AddOption("Severe", "Severe", "Highest threat encounter is severe.")
                .AddOption("Extreme", "Extreme", "Highest threat encounter is extreme.");
        }

        public static SelectMenuBuilder PlayerMaxSelector(ulong guildId, ulong gm, string name)
        {
            Guild guild = Manager.GetGuild(guildId);
            return new SelectMenuBuilder()
                .WithPlaceholder("Max Players")
                .WithCustomId(
                    guild.GenerateFormValues(new($"createQuest", gm, name, "questPlayerMax", new()))
                )
                .WithMinValues(1)
                .AddOption("Four", "4", "No more than four players in the party.")
                .AddOption("Five", "5", "No more than five players in the party.")
                .AddOption("Six", "6", "No more than six players in the party.");
        }

        public static SelectMenuBuilder PlayerMinSelector(ulong guildId, ulong gm, string name)
        {
            Guild guild = Manager.GetGuild(guildId);
            return new SelectMenuBuilder()
                .WithPlaceholder("Min Players")
                .WithCustomId(
                    guild.GenerateFormValues(new($"createQuest", gm, name, "questPlayerMin", new()))
                )
                .WithMinValues(1)
                .AddOption("Two", "2", "No fewer than two players in the party.")
                .AddOption("Three", "3", "No fewer than three players in the party.")
                .AddOption("Four", "4", "No fewer than four players in the party.");
        }

        public static SelectMenuBuilder PartySelector(ulong guildId, ulong gm, string name)
        {
            Guild guild = Manager.GetGuild(guildId);
            SelectMenuBuilder partySelector = new SelectMenuBuilder()
                .WithPlaceholder("Select Party")
                .WithCustomId(
                    guild.GenerateFormValues(new($"editQuest", gm, name, "questPartySelect", new()))
                )
                .WithMinValues(1);
            return partySelector;
        }

        public static ComponentBuilder ConfirmationButtons(
            ulong guildId,
            ulong gm,
            string name,
            string context
        )
        {
            Guild guild = Manager.GetGuild(guildId);
            return new ComponentBuilder()
                .WithButton(
                    "Confirm",
                    guild.GenerateFormValues(new($"{context}", gm, name, "confirm", new())),
                    ButtonStyle.Success
                )
                .WithButton(
                    "Cancel",
                    guild.GenerateFormValues(new($"{context}", gm, name, "cancel", new())),
                    ButtonStyle.Danger
                );
        }

        public ComponentBuilder GenerateComponents()
        {
            return new();
        }

        public async Task DrawQuestPost(SocketMessageComponent component)
        {
            if (component.GuildId == null)
            {
                await component.RespondAsync("This must be run in a guild.", ephemeral: true);
                return;
            }
            Guild guild = Manager.GetGuild(_guild);

            IUser user = component.User;
            if (user.Id != _gameMaster || !guild.IsGamemaster(user))
            {
                await component.RespondAsync("You lack permission to do that!", ephemeral: true);
                return;
            }

            IUser? gm = Manager.GetGuildUser(_guild, _gameMaster);
            if (gm == null)
            {
                await component.RespondAsync($"Unable to find user.", ephemeral: true);
                return;
            }

            await component.UpdateAsync(x =>
            {
                x.Embed = GenerateEmbed(user).Build();
                x.Components = GenerateComponents().Build();
            });
        }

        public async Task DrawQuestPost()
        {
            IThreadChannel? threadChannel = Manager.GetThreadChannel(_guild, ThreadId);
            if (threadChannel == null)
            {
                return;
            }
            IUser? user = Manager.GetGuildUser(_guild, _gameMaster);
            if (user == null)
            {
                return;
            }

            await threadChannel.ModifyMessageAsync(
                MessageId,
                msg =>
                {
                    msg.Embed = GenerateEmbed(user).Build();
                    msg.Components = GenerateComponents().Build();
                    msg.Content = string.Empty;
                }
            );
        }
    }
}
