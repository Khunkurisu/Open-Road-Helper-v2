using Bot.Guilds;
using Bot.Helpers;
using Discord;

namespace Bot.Characters
{
    public partial class Character
    {
        private readonly Guid _id;
        private readonly ulong _user;
        private readonly ulong _guild;

        private string _name;
        private string _desc;
        private string _rep;
        private uint _age;
        private string _deity;
        private string _gender;
        private float _height;
        private float _weight;
        private int _birthDay;
        private Months _birthMonth;
        private int _birthYear;

        private readonly string _ancestry;
        private readonly string _heritage;
        private readonly string _class;
        private readonly string _background;
        private uint _level = 0;
        private int _generation = 1;
        private double _currency = 0;
        private int _downtime = 0;

        private List<string> _languages;
        private Dictionary<string, uint> _skills;
        private Dictionary<string, uint> _lore;
        private Dictionary<string, uint> _saves;
        private uint _perception;
        private List<string> _feats;
        private List<string> _spells;
        private List<string> _edicts = new() { "" };
        private List<string> _anathema = new() { "" };
        private Dictionary<string, uint> _attributes =
            new()
            {
                { "str", 10 },
                { "dex", 10 },
                { "con", 10 },
                { "int", 10 },
                { "wis", 10 },
                { "cha", 10 }
            };

        private string _colorPref = "#000000";
        private readonly List<string> _avatars = new() { "" };
        private readonly List<string> _notes = new() { "" };

        private int _lastTokenTrade = 0;
        private ulong _messageId;
        private readonly long _created;
        private long _updated = 0;
        private Status _status = 0;

        public Character(
            IUser user,
            Guild guild,
            string name,
            string desc,
            string rep,
            Dictionary<string, dynamic> data
        )
        {
            ImportType importType = data["type"];
            if (importType == ImportType.Foundry)
            {
                _height = GenericHelpers.GetHeightFromString(data["height"]);
                _weight = GenericHelpers.GetWeightFromString(data["weight"]);
            }
            _id = Guid.NewGuid();
            _user = user.Id;
            _guild = guild.Id;
            _name = name;
            _desc = desc;
            _rep = rep;
            _created = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            _level = guild.Generation;
            _generation = guild.GenerationCount;

            _class = data["class"];
            _ancestry = data["ancestry"];
            _heritage = data["heritage"];
            _background = data["background"];
            _deity = data["deity"];
            _gender = data["gender"];
            _age = data["age"];
            int perception = data["perception"];
            _perception = (uint)perception;
            _currency = data["coin"];
            _languages = data["languages"];
            _skills = data["skills"];
            _lore = data["lore"];
            _saves = data["saves"];
            _feats = data["feats"];
            _spells = data["spells"];
            _attributes = data["attributes"];

            if (data.ContainsKey("edicts"))
            {
                _edicts = data["edicts"];
            }

            if (data.ContainsKey("anathema"))
            {
                _anathema = data["anathema"];
            }
        }

        private void SetStatus(Status status)
        {
            _status = status;
            _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        private void SetFeatsList(List<string> newFeats)
        {
            _feats = newFeats;
            _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        private void SetSpellsList(List<string> newSpells)
        {
            _spells = newSpells;
            _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        private void SetEdictsList(List<string> newEdicts)
        {
            _edicts = newEdicts;
            _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        private void SetAnathemaList(List<string> newAnathema)
        {
            _anathema = newAnathema;
            _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

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
            get => _name;
            set
            {
                _name = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public string Desc
        {
            get => _desc;
            set
            {
                _desc = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public string Rep
        {
            get => _rep;
            set
            {
                _rep = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        public string Deity
        {
            get => _deity;
            set
            {
                _deity = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        public string Gender
        {
            get => _gender;
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
            get => _ancestry;
        }
        public string Heritage
        {
            get => _heritage;
        }
        public string Class
        {
            get => _class;
        }
        public string Background
        {
            get => _background;
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
        public string AvatarURL
        {
            get { return _avatars[0]; }
            set
            {
                _avatars[0] = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public ulong MessageId
        {
            get => _messageId;
            set
            {
                _messageId = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public List<string> Notes
        {
            get => _notes;
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
            set { SetStatus(value); }
        }
        public List<string> Feats
        {
            get => _feats;
            set { SetFeatsList(value); }
        }
        public List<string> Spells
        {
            get => _spells;
            set { SetSpellsList(value); }
        }
        public List<string> Edicts
        {
            get => _edicts;
            set { SetEdictsList(value); }
        }
        public List<string> Anathema
        {
            get => _anathema;
            set { SetAnathemaList(value); }
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
                    uint score = _attributes[attribute];
                    int mod = (int)MathF.Round((score - 10) / 2);
                    mods[attribute] = mod;
                }
                return mods;
            }
        }
    }
}
