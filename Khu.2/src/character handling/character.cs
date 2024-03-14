using Bot.Helpers;
using Discord;

namespace Bot.Characters
{
    public class Character
    {
        public static int Total = 0;
        private readonly int _id;
        private readonly IUser _user;

        private string _name;
        private string _desc;
        private string _rep;
        private int _age;
        private float _height;
        private float _weight;
        private int _birthDay;
        private Months _birthMonth;
        private int _birthYear;

        private Ancestry _ancestry;
        private Heritage _heritage;
        private Class _class;
        private int _level;
        private Generation _generation;
        private float _currency;
        private int _downtime;

        private string _colorPref;
        private List<string> _avatars;
        private string _charSheet;
        private List<string> _notes = new();

        private int _lastTokenTrade = 0;
        private ulong _messageId;
        private readonly long _created;
        private long _updated;
        private int _status = 0;

        public Character(
            IUser user,
            string name,
            string desc,
            string rep,
            int age,
            float height,
            float weight,
            int birthday,
            Months birthmonth,
            int birthyear,
            Ancestry ancestry,
            Heritage heritage,
            Class charClass,
            int level,
            Generation generation,
            float currency,
            int downtime,
            string color,
            List<string> avatars,
            string sheet,
            ulong messageId
        )
        {
            Total++;
            _id = Total;
            _user = user;
            _name = name;
            _desc = desc;
            _rep = rep;
            _age = age;
            _height = height;
            _weight = weight;
            _ancestry = ancestry;
            _heritage = heritage;
            _class = charClass;
            _level = level;
            _currency = currency;
            _downtime = downtime;
            _colorPref = color;
            _avatars = avatars;
            _charSheet = sheet;
            _messageId = messageId;
            _generation = generation;
            _birthDay = birthday;
            _birthMonth = birthmonth;
            _birthYear = birthyear;
            _created = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        private void SetStatus(Status status)
        {
            _status = (int)status;
        }

        public void AddNote(string note)
        {
            _notes.Add(note);
        }

        public void RemoveNote(int index)
        {
            if (index > 0 && index < _notes.Count - 1)
            {
                _notes.RemoveAt(index);
            }
        }

        public string GetBirthDate()
        {
            return $"{GenericHelpers.Ordinal(_birthDay)} {_birthMonth} {_birthYear} AR";
        }

        public int Id
        {
            get { return _id; }
        }
        public IUser User
        {
            get { return _user; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Desc
        {
            get { return _desc; }
            set { _desc = value; }
        }
        public string Rep
        {
            get { return _rep; }
            set { _rep = value; }
        }
        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }
        public float Height
        {
            get { return _height; }
            set { _height = value; }
        }
        public float Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }
        public int Birthday
        {
            get { return _birthDay; }
        }
        public Months BirthMonth
        {
            get { return _birthMonth; }
        }
        public int BirthYear
        {
            get { return _birthYear; }
        }
        public Ancestry Ancestry
        {
            get { return _ancestry; }
        }
        public Heritage Heritage
        {
            get { return _heritage; }
        }
        public Class Class
        {
            get { return _class; }
        }
        public Generation Generation
        {
            get { return _generation; }
        }
        public int Level
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
            set { _colorPref = value; }
        }
        public string AvatarURL
        {
            get { return _avatars[0]; }
            set { _avatars[0] = value; }
        }
        public string SheetURL
        {
            get { return _charSheet; }
            set { _charSheet = value; }
        }
        public ulong MessageId
        {
            get { return _messageId; }
            set { _messageId = value; }
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
