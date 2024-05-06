using Bot.Helpers;
using Discord;

namespace Bot.Characters
{
    public class Character
    {
        private readonly Guid _id;
        private readonly ulong _user;

        private string _name;
        private string _desc;
        private string _rep;
        private uint _age;
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
        private uint _generation = 1;
        private float _currency = 0;
        private int _downtime = 0;

        private List<string> _languages;
        private Dictionary<string, uint> _skills;
        private Dictionary<string, uint> _lore;
        private Dictionary<string, uint> _saves;
        private List<string> _feats;
        private List<string> _spells;
        private List<string> _edicts = new();
        private List<string> _anathema = new();

        private string _colorPref = "#000000";
        private readonly List<string> _avatars = new();
        private readonly List<string> _notes = new();

        private int _lastTokenTrade = 0;
        private ulong _messageId;
        private readonly long _created;
        private long _updated = 0;
        private int _status = 0;

        public Character(
            IUser user,
            string name,
            string desc,
            string rep,
            Dictionary<string, dynamic> data
        )
        {
            _id = Guid.NewGuid();
            _user = user.Id;
            _name = name;
            _desc = desc;
            _rep = rep;
            _created = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            _class = data["class"];
            _ancestry = data["ancestry"];
            _heritage = data["heritage"];
            _background = data["background"];
            _age = data["age"];
            _currency = data["coin"];
            _languages = data["languages"];
            _skills = data["skills"];
            _lore = data["lore"];
            _saves = data["saves"];
            _feats = data["feats"];
            _spells = data["spells"];

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
            _status = (int)status;
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
            get { return _id; }
        }
        public ulong User
        {
            get { return _user; }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public string Desc
        {
            get { return _desc; }
            set
            {
                _desc = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public string Rep
        {
            get { return _rep; }
            set
            {
                _rep = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public uint Age
        {
            get { return _age; }
            set
            {
                _age = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public float Height
        {
            get { return _height; }
            set
            {
                _height = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public float Weight
        {
            get { return _weight; }
            set
            {
                _weight = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public int Birthday
        {
            get { return _birthDay; }
            set
            {
                _birthDay = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public Months BirthMonth
        {
            get { return _birthMonth; }
            set
            {
                _birthMonth = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public int BirthYear
        {
            get { return _birthYear; }
            set
            {
                _birthYear = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public string Ancestry
        {
            get { return _ancestry; }
        }
        public string Heritage
        {
            get { return _heritage; }
        }
        public string Class
        {
            get { return _class; }
        }
        public string Background
        {
            get { return _background; }
        }
        public uint Generation
        {
            get { return _generation; }
        }
        public uint Level
        {
            get { return _level; }
        }
        public float Gold
        {
            get { return _currency; }
        }
        public int Downtime
        {
            get { return _downtime; }
        }
        public string Color
        {
            get { return _colorPref; }
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
            get { return _messageId; }
            set
            {
                _messageId = value;
                _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
        public List<string> Notes
        {
            get { return _notes; }
        }
        public int LastTokenTrade
        {
            get { return _lastTokenTrade; }
        }
        public long CreatedOn
        {
            get { return _created; }
        }
        public long UpdatedOn
        {
            get { return _updated; }
        }
        public int Status
        {
            get { return _status; }
            set { SetStatus((Status)value); }
        }
    }
}
