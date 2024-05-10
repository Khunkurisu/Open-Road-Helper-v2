using Bot.Guilds;
using Discord;

namespace Bot.Characters
{
    public partial class Character
    {
        public EmbedBuilder? GenerateEmbed()
        {
            Guild guild = BotManager.GetGuild(_guild);
            IUser? player = BotManager.GetGuildUser(_guild, _user);
            if (player == null)
            {
                return null;
            }

            var embed = new EmbedBuilder()
                .WithTitle(Heritage + " " + Ancestry + " " + Class + " " + Level)
                .WithAuthor(Name)
                .WithDescription(Desc)
                .WithFooter(player.Username)
                .AddField("Reputation", Rep, false)
                .AddField("Age", Age, true)
                .AddField("Height", Height + " cm", true)
                .AddField("Weight", Weight + " kg", true)
                .AddField("PT", guild.GetPlayerTokenCount(User).ToString(), true)
                .AddField("DT", Downtime.ToString(), true)
                .AddField("Coin", Gold + " gp", true)
                .WithImageUrl(AvatarURL);

            return embed;
        }
    }
}
