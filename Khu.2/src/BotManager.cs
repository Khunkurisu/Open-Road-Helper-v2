using Bot.Guilds;

namespace Bot
{
    public class BotManager
    {
        public static readonly Dictionary<ulong, Guild> Guilds = new();

        public static Guild GetGuildById(ulong id)
        {
            return Guilds.ContainsKey(id) ? Guilds[id] : new(id);
        }
    }
}
