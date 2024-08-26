namespace OpenRoadHelper.Characters
{
    public partial class Character
    {
        private readonly Guid _id;
        private readonly ulong _user;
        private readonly ulong _guild;
        private ulong _characterThread;
        private ulong _transactionThread;

        private string? _name;
        private string? _desc;
        private string? _rep;
        private uint _age = 18;
        private string? _deity;
        private string? _gender;
        private float _height = 100;
        private float _weight = 80;
        private int _birthDay = 1;
        private PF2E.Months _birthMonth;
        private int _birthYear = 4706;

        private string? _ancestry;
        private string? _heritage;
        private string? _class;
        private string? _background;

        private uint _level = 0;
        private int _generation = 1;
        private double _currency = 0;
        private uint _downtime = 0;

        private Dictionary<string, int> _skills = new() { };
        private Dictionary<string, int> _lore = new() { };
        private Dictionary<string, int> _saves = new() { };
        private uint _perception = 0;

        private List<string> _languages = new() { "" };
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
        private Dictionary<string, string> _avatars = new() { };
        private List<string> _notes = new() { };

        private int _lastTokenTrade = 0;
        private readonly long _created;
        private long _updated = 0;
        private Status _status = 0;
        private Display _display = 0;
        private int _currentAvatar = 0;
        private const string _avatarPrefix = @".\data\images\characters\";
    }
}
