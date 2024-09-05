using OpenRoadHelper.Guilds;
using Discord;

namespace OpenRoadHelper.Quests
{
    public partial class Quest
    {
        public EmbedBuilder GenerateEmbed(IUser gm)
        {
            var embed = new EmbedBuilder()
                .WithTitle(Name)
                .WithAuthor(gm.Username)
                .WithDescription(Description)
                .AddField("Threat", Threat.ToString(), true)
                .AddField("Player Count", MinPlayers + " to " + MaxPlayers, true)
                .WithImageUrl(Image);

            Guild guild = Manager.GetGuild(_guild);
            Party? selectedParty = guild.GetParty(SelectedParty);
            if (selectedParty != null)
            {
                embed.AddField(selectedParty.Name, selectedParty.Members);
            }

            if (Tags.Any())
            {
                embed.AddField("Tags", Tags);
            }

            return embed;
        }

        public EmbedBuilder? GenerateEmbed()
        {
            IUser? gm = Manager.GetGuildUser(_guild, _gameMaster);
            if (gm != null)
            {
                return GenerateEmbed(gm);
            }
            return null;
        }
    }
}
