namespace OpenRoadHelper.Quests
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
        public ulong GuildId
        {
            get => _guild;
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
        public List<string> TagsList
        {
            get => _tags;
            set
            {
                _tags = value;
                _lastUpdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public string Tags => string.Join(", ", TagsList);
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
        public ulong MessageId
        {
            get => _message;
            set
            {
                _message = value;
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

        public string? Image => Images.Any() ? Images[(int)SelectedImage] : string.Empty;

        public List<string> Images
        {
            get => _images;
        }

        public int SelectedImage
        {
            get => (int)(_selectedImage + 1 <= _images.Count ? _selectedImage : 0);
            set { _selectedImage = (uint)(_images.Any() ? Math.Min(value, _images.Count - 1) : 0); }
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
