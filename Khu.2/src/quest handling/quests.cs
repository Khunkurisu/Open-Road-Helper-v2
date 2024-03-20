using Bot.Characters;
using Bot.PF2;
using Discord;

namespace Bot.Quests
{
    public class Quest
    {
        private static uint _total = 0;
        private readonly Guid _id;
        private string _name = "temp";
        private ulong _gm;
        private string _desc = "temp";
        private int _threat;
        private readonly List<string> _tags = new();
        private int _maxPlayers;
        private readonly List<Character> _chars = new();
        private int _status = 0;
        private ulong _messageId;
        private ulong _threadId;
        private readonly long _created;

        public Quest()
        {
            _total++;
            _id = Guid.NewGuid();
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
            _total++;
            _id = Guid.NewGuid();
            _name = name;
            _gm = gm;
            _desc = desc;
            _threat = threat;
            _tags = tags;
            _maxPlayers = maxPlayers;
            _messageId = messageId;
            _threadId = threadId;
            _created = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
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
            _total++;
            _id = Guid.NewGuid();
            _name = name;
            _gm = gm;
            _desc = desc;
            _threat = threat;
            _tags = tags;
            _maxPlayers = maxPlayers;
        }

        public Quest(ulong gm, string name, string desc, List<string> tags)
        {
            _total++;
            _id = Guid.NewGuid();
            _name = name;
            _gm = gm;
            _desc = desc;
            _tags = tags;
        }

        public string GetTags()
        {
            return string.Join(", ", Tags);
        }

        public void SetStatus(Status status)
        {
            _status = (int)status;
        }

        public static uint Total
        {
            get { return _total; }
        }

        public Guid Id
        {
            get => _id;
        }
        public string Name
        {
            get => _name;
            set => _name = value;
        }
        public ulong GM
        {
            get => _gm;
        }
        public string Desc
        {
            get => _desc;
            set => _desc = value;
        }
        public Helper.Threats Threat
        {
            get => (Helper.Threats)_threat;
            set { _threat = (int)value; }
        }
        public List<string> Tags
        {
            get => _tags;
        }
        public int MaxPlayers
        {
            get => _maxPlayers;
            set => _maxPlayers = value;
        }
        public List<Character> Characters
        {
            get => _chars;
        }
        public int Status
        {
            get => _status;
        }
        public ulong MessageId
        {
            get => _messageId;
            set => _messageId = value;
        }
        public ulong ThreadId
        {
            get => _threadId;
            set => _threadId = value;
        }
        public long Created
        {
            get => _created;
        }

        public EmbedBuilder GenerateEmbed(IUser gm)
        {
            var embed = new EmbedBuilder()
                .WithTitle(Name)
                .WithAuthor(gm.Username)
                .WithDescription(Desc)
                .WithFooter(GetTags())
                .AddField("Threat", Threat.ToString(), true)
                .AddField("Player Count", Characters.Count + "/" + MaxPlayers, true);

            if (Characters.Count > 0)
            {
                for (int i = 0; i < Characters.Count - 1; i++)
                {
                    embed.AddField("Character " + (i + 1), Characters[i].Name, true);
                }
            }

            return embed;
        }

        public static SelectMenuBuilder ThreatSelector(string guildId, string gm, string name)
        {
            return new SelectMenuBuilder()
                .WithPlaceholder("Quest Threat")
                .WithCustomId("createQuest-" + guildId + "-" + gm + "-" + name + "-questThreat")
                .WithMinValues(1)
                .AddOption("Trivial", "Trivial", "Highest threat encounter is trivial.")
                .AddOption("Low", "Low", "Highest threat encounter is low.")
                .AddOption("Moderate", "Moderate", "Highest threat encounter is moderate.")
                .AddOption("Severe", "Severe", "Highest threat encounter is severe.")
                .AddOption("Extreme", "Extreme", "Highest threat encounter is extreme.");
        }

        public static SelectMenuBuilder PlayerMaxSelector(string guildId, string gm, string name)
        {
            return new SelectMenuBuilder()
                .WithPlaceholder("Max Players")
                .WithCustomId("createQuest-" + guildId + "-" + gm + "-" + name + "-questPlayerMax")
                .WithMinValues(1)
                .AddOption("Three", "3", "No more than three players in the party.")
                .AddOption("Four", "4", "No more than four players in the party.")
                .AddOption("Five", "5", "No more than five players in the party.")
                .AddOption("Six", "6", "No more than six players in the party.");
        }

        public static ComponentBuilder ConfirmationButtons(string guildId, string gm, string name)
        {
            return new ComponentBuilder()
                .WithButton(
                    "Confirm",
                    "createQuest-" + guildId + "-" + gm + "-" + name + "-confirm",
                    ButtonStyle.Success
                )
                .WithButton(
                    "Cancel",
                    "createQuest-" + guildId + "-" + gm + "-" + name + "-cancel",
                    ButtonStyle.Danger
                );
        }

        public void SetMaxPlayers(string maxPlayers)
        {
            _maxPlayers = int.Parse(maxPlayers);
        }

        public void SetThreat(string threat)
        {
            if (Enum.TryParse(threat, out Helper.Threats threats))
            {
                _threat = (int)threats;
            }
        }
    }
}
