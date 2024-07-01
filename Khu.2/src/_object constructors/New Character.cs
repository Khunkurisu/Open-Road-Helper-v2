using Bot.Guilds;
using Discord;

namespace Bot.Characters
{
    public partial class Character
    {
        public Character(IUser user, Guild guild, Dictionary<string, dynamic> data)
        {
            _id = Guid.NewGuid();
            _user = user.Id;
            _guild = guild.Id;
            _created = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            _level = guild.Generation;
            _generation = guild.GenerationCount;

            ParseImport(data);
        }
    }
}
