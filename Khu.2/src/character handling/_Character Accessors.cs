using Bot.Guilds;
using Bot.Helpers;
using Bot.PF2;

namespace Bot.Characters
{
    public partial class Character
    {
        public void AddNote(string note)
        {
            _notes.Add(note);
            _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public void RemoveNote(int index)
        {
            if (index > 0 && index < _notes.Count - 1)
            {
                _notes.RemoveAt(index);
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        public string GetBirthDate()
        {
            return $"{GenericHelpers.Ordinal(_birthDay)} {_birthMonth} {_birthYear} AR";
        }

        public Guid Id
        {
            get => _id;
        }
        public ulong User
        {
            get => _user;
        }
        public ulong Guild
        {
            get => _guild;
        }
        public string Name
        {
            get => _name ?? string.Empty;
            set
            {
                _name = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public string Description
        {
            get => _desc ?? string.Empty;
            set
            {
                _desc = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public string Reputation
        {
            get => _rep ?? string.Empty;
            set
            {
                _rep = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        public string Deity
        {
            get => _deity ?? string.Empty;
            set
            {
                _deity = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        public string Gender
        {
            get => _gender ?? string.Empty;
            set
            {
                _gender = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public uint Age
        {
            get => _age;
            set
            {
                _age = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public float Height
        {
            get => MathF.Truncate(_height * 100) / 100;
            set
            {
                _height = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public float Weight
        {
            get => MathF.Truncate(_weight * 100) / 100;
            set
            {
                _weight = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public int Birthday
        {
            get => _birthDay;
            set
            {
                _birthDay = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public Months BirthMonth
        {
            get => _birthMonth;
            set
            {
                _birthMonth = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public int BirthYear
        {
            get => _birthYear;
            set
            {
                _birthYear = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public string Ancestry
        {
            get => _ancestry ?? string.Empty;
        }
        public string Heritage
        {
            get => _heritage ?? string.Empty;
        }
        public string Class
        {
            get => _class ?? string.Empty;
        }
        public string Background
        {
            get => _background ?? string.Empty;
        }
        public int Generation
        {
            get => _generation;
            set
            {
                _generation = value;
                Guild guild = BotManager.GetGuild(_guild);
                _level = guild.Generations[_generation];
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public uint Level
        {
            get => _level;
        }
        public double Gold
        {
            get => Math.Truncate(_currency * 100) / 100;
            set
            {
                _currency = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public Display DisplayMode
        {
            get => _display;
            set
            {
                _display = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        public ulong CharacterThread
        {
            get => _characterThread;
            set
            {
                _characterThread = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        public ulong TransactionThread
        {
            get => _transactionThread;
            set
            {
                _transactionThread = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public int Downtime
        {
            get => _downtime;
            set
            {
                _downtime = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public string Color
        {
            get => _colorPref;
            set
            {
                _colorPref = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public List<string> Avatars
        {
            get => _avatars;
            set
            {
                _avatars = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public List<string> Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public int LastTokenTrade
        {
            get => _lastTokenTrade;
            set
            {
                _lastTokenTrade = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public long CreatedOn
        {
            get => _created;
        }
        public long UpdatedOn
        {
            get => _updated;
        }
        public Status Status
        {
            get => _status;
            set
            {
                _status = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public List<string> Feats
        {
            get => _feats;
            set
            {
                _feats = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public List<string> Spells
        {
            get => _spells;
            set
            {
                _spells = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public List<string> Edicts
        {
            get => _edicts;
            set
            {
                _edicts = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public List<string> Anathema
        {
            get => _anathema;
            set
            {
                _anathema = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        public List<string> Languages
        {
            get => _languages;
            set
            {
                _languages = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public Dictionary<string, uint> Skills
        {
            get => _skills;
            set
            {
                _skills = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public Dictionary<string, uint> Lore
        {
            get => _lore;
            set
            {
                _lore = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public Dictionary<string, uint> Saves
        {
            get => _saves;
            set
            {
                _saves = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        public uint Perception
        {
            get => _perception;
            set
            {
                _perception = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        public Dictionary<string, uint> Attributes
        {
            get => _attributes;
            set
            {
                _attributes = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        public Dictionary<string, int> Modifiers
        {
            get
            {
                Dictionary<string, int> mods =
                    new()
                    {
                        { "str", 0 },
                        { "dex", 0 },
                        { "con", 0 },
                        { "int", 0 },
                        { "wis", 0 },
                        { "cha", 0 }
                    };
                foreach (string attribute in _attributes.Keys)
                {
                    mods[attribute] = Helper.AttributeToModifier(_attributes[attribute]);
                }
                return mods;
            }
        }
    }

    public enum Status
    {
        Temp,
        Pending,
        Approved,
        Retired,
        Deceased,
        Rejected
    }
}
