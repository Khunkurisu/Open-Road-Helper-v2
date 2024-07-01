namespace Bot.Quests
{
    public partial class Quest
    {
        public Guid Id
        {
            get => _id;
        }
        public string Name
        {
            get => _name;
        }
        public ulong GameMaster
        {
            get => _gameMaster;
        }
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                _lastUpdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public Threats Threat
        {
            get => _threat;
            set
            {
                _threat = value;
                _lastUpdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public List<string> Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                _lastUpdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public int MinPlayers
        {
            get => _minPlayers;
            set
            {
                _minPlayers = value;
                _lastUpdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public int MaxPlayers
        {
            get => _maxPlayers;
            set
            {
                _maxPlayers = value;
                _lastUpdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public List<Guid> Parties
        {
            get => _parties;
            set
            {
                _parties = value;
                _lastUpdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public Guid? SelectedParty
        {
            get => _selectedParty;
            set
            {
                _selectedParty = value;
                _lastUpdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public Status Status
        {
            get => _status;
            set
            {
                _status = value;
                _lastUpdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public ulong ThreadId
        {
            get => _thread;
            set
            {
                _thread = value;
                _lastUpdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public long CreatedAt
        {
            get => _createdAt;
        }
        public long LastUpdated
        {
            get => _lastUpdated;
        }

        public string GetTags()
        {
            return string.Join(", ", Tags);
        }

        public void SetStatus(Status status)
        {
            Status = status;
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
                Threat = threats;
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
