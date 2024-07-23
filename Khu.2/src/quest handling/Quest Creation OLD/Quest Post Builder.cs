using Bot.Guilds;
using Discord;

namespace Bot.Quests
{
    public partial class Quest
    {
        public EmbedBuilder GenerateEmbed(IUser gm)
        {
            var embed = new EmbedBuilder()
                .WithTitle(Name)
                .WithAuthor(gm.Username)
                .WithDescription(Description)
                .WithFooter(GetTags())
                .AddField("Threat", Threat.ToString(), true)
                .AddField("Player Count", MinPlayers + " to " + MaxPlayers, true);

            return embed;
        }

        public static SelectMenuBuilder ThreatSelector(ulong guildId, ulong gm, string name)
        {
            Guild guild = Manager.GetGuild(guildId);
            return new SelectMenuBuilder()
                .WithPlaceholder("Quest Threat")
                .WithCustomId(
                    guild.GenerateFormValues(new() { $"createQuest", gm, name, "questThreat" })
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
                    guild.GenerateFormValues(new() { $"createQuest", gm, name, "questPlayerMax" })
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
                    guild.GenerateFormValues(new() { $"createQuest", gm, name, "questPlayerMin" })
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
                    guild.GenerateFormValues(new() { $"editQuest", gm, name, "questPartySelect" })
                )
                .WithMinValues(1);
            return partySelector;
        }

        public static ComponentBuilder ConfirmationButtons(ulong guildId, ulong gm, string name)
        {
            Guild guild = Manager.GetGuild(guildId);
            return new ComponentBuilder()
                .WithButton(
                    "Confirm",
                    guild.GenerateFormValues(new() { $"createQuest", gm, name, "confirm" }),
                    ButtonStyle.Success
                )
                .WithButton(
                    "Cancel",
                    guild.GenerateFormValues(new() { $"createQuest", gm, name, "cancel" }),
                    ButtonStyle.Danger
                );
        }
    }
}
