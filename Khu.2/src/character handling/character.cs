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
        private ulong _thread;

        private string? _name;
        private string? _desc;
        private string? _rep;
        private uint _age = 18;
        private string? _deity;
        private string? _gender;
        private float _height = 100;
        private float _weight = 80;
        private int _birthDay = 1;
        private Months _birthMonth;
        private int _birthYear = 4706;

        private string? _ancestry;
        private string? _heritage;
        private string? _class;
        private string? _background;
        private uint _level = 0;
        private int _generation = 1;
        private double _currency = 0;
        private int _downtime = 0;

        private List<string> _languages = new() { "" };
        private Dictionary<string, uint> _skills = new() { };
        private Dictionary<string, uint> _lore = new() { };
        private Dictionary<string, uint> _saves = new() { };
        private uint _perception = 0;
        private List<string> _feats = new() { "" };
        private List<string> _spells = new() { "" };
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
        private readonly long _created;
        private long _updated = 0;
        private Status _status = 0;
        private Display _display = 0;

        public Character(Character template)
        {
            _id = template.Id;
            _user = template.User;
            _guild = template.Guild;
            _created = template.CreatedOn;
            _level = template.Level;
            _generation = template.Generation;

            _name = template.Name;
            _desc = template.Description;
            _rep = template.Reputation;
            _class = template.Class;
            _ancestry = template.Ancestry;
            _heritage = template.Heritage;
            _background = template.Background;
            _deity = template.Deity;
            _gender = template.Gender;
            _age = template.Age;
            _perception = template.Perception;
            _currency = template.Gold;
            _languages = template.Languages;
            _skills = template.Skills;
            _lore = template.Lore;
            _saves = template.Saves;
            _feats = template.Feats;
            _spells = template.Spells;
            _attributes = template.Attributes;
        }

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

        public ulong Thread
        {
            get => _thread;
            set
            {
                _thread = value;
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
                    uint score = _attributes[attribute];
                    int mod = (int)MathF.Round((score - 10) / 2);
                    mods[attribute] = mod;
                }
                return mods;
            }
        }
    }
}
