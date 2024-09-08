using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OpenRoadHelper.Guilds;

namespace OpenRoadHelper.Characters
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

		public bool ShouldSave => Status != Status.Replacement && Status != Status.Temp;

        public string GetBirthDate()
        {
            return $"{Generic.Ordinal(_birthDay)} {_birthMonth} {_birthYear} AR";
        }

        public string Id
        {
            get => Generic.GuidToBase64(_id);
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
            get
            {
                string descParsed = _desc ?? string.Empty;
                descParsed = descParsed.Replace("<br>", "\n");
                descParsed = descParsed.Replace("<br />", "\n");
                descParsed = descParsed.Replace("<p>", "");
                descParsed = descParsed.Replace("</p>", "\n");
                descParsed = descParsed.Replace("<ul>", "");
                descParsed = descParsed.Replace("<ol>", "");
                descParsed = descParsed.Replace("<li>", "* ");
                return descParsed;
            }
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
        public PF2E.Months BirthMonth
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
                Guild guild = Manager.GetGuild(_guild);
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

        [JsonIgnore]
        public DateTime AwaitingInput { get; set; } = DateTime.MinValue;

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
        public uint Downtime
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
        public Dictionary<string, string> Avatars
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

        [JsonConverter(typeof(StringEnumConverter))]
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
        public Dictionary<string, int> Skills
        {
            get => _skills;
            set
            {
                _skills = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public Dictionary<string, int> Lore
        {
            get => _lore;
            set
            {
                _lore = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public Dictionary<string, int> Saves
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
                    mods[attribute] = PF2E.AttributeToModifier(_attributes[attribute]);
                }
                return mods;
            }
        }

        public int Avatar
        {
            get => _currentAvatar;
            set
            {
                _currentAvatar = Math.Clamp(value, 0, Avatars.Count);
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        public void NextAvatar()
        {
            Avatar++;
        }

        public void PreviousAvatar()
        {
            Avatar--;
        }

        public bool CheckMoreAvatars(bool forward = true)
        {
            if (forward)
            {
                return Avatar >= Avatars.Count - 1;
            }
            return Avatars.Count < 1 || Avatar == 0;
        }

        public string GetAvatar(int index)
        {
            if (Avatars.Any())
            {
                if (Avatars.Count - 1 >= index)
                {
                    return $@"{AvatarPrefix}{Avatars[Avatars.Keys.ElementAt(index)]}";
                }
                else
                {
                    return $@"{AvatarPrefix}{Avatars[Avatars.Keys.ElementAt(0)]}";
                }
            }
            return string.Empty;
        }

        public string GetAvatar(string key)
        {
            if (Avatars.Any())
            {
                if (Avatars.ContainsKey(key))
                {
                    return $"{AvatarPrefix}{Avatars[key]}";
                }
                else
                {
                    return $"{AvatarPrefix}{Avatars.ElementAt(0)}";
                }
            }
            return string.Empty;
        }

        public string GetAvatar()
        {
            return GetAvatar(Avatar);
        }

        public int GetAvatarIndex(string key)
        {
            return Avatars.Keys.ToList().IndexOf(key);
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public Status RetirementType = Status.Retired;

        [JsonConverter(typeof(StringEnumConverter))]
        public ImportType ImportType => _importType;

        [JsonProperty]
        public static float SaveVersion => _saveVersion;
        public static readonly string AvatarPrefix =
            "http://pf2.khunkurisu.com/open-road/character-images/";
    }

    public enum Status
    {
        Temp,
        Pending,
        Approved,
        Retired,
        Deceased,
        Rejected,
        Replacement
    }
}
