namespace OpenRoadHelper.Quests
{
    public partial class Quest
    {
        public Quest(ulong guild)
        {
            _id = Guid.NewGuid();
            _guild = guild;
            _createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public Quest(
            ulong gm,
            ulong guild,
            string name,
            string desc,
            int threat,
            int maxPlayers,
            List<string> tags,
            ulong threadId
        )
        {
            _id = Guid.NewGuid();
            _name = name;
            _guild = guild;
            _gameMaster = gm;
            _description = desc;
            _threat = (Threats)threat;
            _tags = tags;
            _maxPlayers = maxPlayers;
            _thread = threadId;
            _createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public Quest(
            ulong gm,
            ulong guild,
            string name,
            string desc,
            int threat,
            int maxPlayers,
            List<string> tags
        )
        {
            _id = Guid.NewGuid();
            _guild = guild;
            _name = name;
            _gameMaster = gm;
            _description = desc;
            _threat = (Threats)threat;
            _tags = tags;
            _maxPlayers = maxPlayers;
            _createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public Quest(ulong gm, ulong guild, string name, string desc, List<string> tags)
        {
            _id = Guid.NewGuid();
            _guild = guild;
            _name = name;
            _gameMaster = gm;
            _description = desc;
            _tags = tags;
            _createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}
