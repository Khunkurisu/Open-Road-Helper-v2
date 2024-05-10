using Bot.Guilds;
using Discord;
using Newtonsoft.Json;

namespace Bot.Quests
{
    public partial class Quest
    {
        public Guid Id;
        public string Name = "temp";
        public ulong GameMaster;
        public string Description = "temp";
        public int Threat;
        public List<string> Tags = new();
        public int MinPlayers;
        public int MaxPlayers;
        public List<Guid> Parties = new();
        public Guid? selectedParty;
        public int Status = 0;
        public ulong MessageId;
        public ulong ThreadId;
        public DateTime CreatedAt;
        public DateTime LastUpdated;

        public Quest()
        {
            Id = Guid.NewGuid();
        }

        public Quest(
            ulong gm,
            string name,
            string desc,
            int threat,
            int maxPlayers,
            List<string> tags,
            ulong messageId,
            ulong threadId
        )
        {
            Id = Guid.NewGuid();
            Name = name;
            GameMaster = gm;
            Description = desc;
            Threat = threat;
            Tags = tags;
            MaxPlayers = maxPlayers;
            MessageId = messageId;
            ThreadId = threadId;
            CreatedAt = DateTime.Now;
        }

        public Quest(
            ulong gm,
            string name,
            string desc,
            int threat,
            int maxPlayers,
            List<string> tags
        )
        {
            Id = Guid.NewGuid();
            Name = name;
            GameMaster = gm;
            Description = desc;
            Threat = threat;
            Tags = tags;
            MaxPlayers = maxPlayers;
        }

        public Quest(ulong gm, string name, string desc, List<string> tags)
        {
            Id = Guid.NewGuid();
            Name = name;
            GameMaster = gm;
            Description = desc;
            Tags = tags;
        }

        public string GetTags()
        {
            return string.Join(", ", Tags);
        }

        public void SetStatus(Status status)
        {
            Status = (int)status;
        }

        public void SetMaxPlayers(string maxPlayers)
        {
            MaxPlayers = int.Parse(maxPlayers);
        }

        public void SetMinPlayers(string minPlayers)
        {
            MinPlayers = int.Parse(minPlayers);
        }

        public void SetThreat(string threat)
        {
            if (Enum.TryParse(threat, out Threats threats))
            {
                Threat = (int)threats;
            }
        }
    }

    public enum Status
    {
        Unplayed,
        Ongoing,
        Completed
    }

    public enum Threats
    {
        Trivial,
        Low,
        Moderate,
        Severe,
        Extreme
    }
}
