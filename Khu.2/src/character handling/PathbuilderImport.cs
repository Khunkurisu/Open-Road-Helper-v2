namespace Bot.Characters
{
    public class PathbuilderImport : IImportable
    {
        public PathbuilderImport(Dictionary<string, dynamic> jsonData)
        {
            // some code here
        }

        private readonly string _name;
        private string _description = "";
        private string _backstory = "";
        private string _birthplace = "";
        private uint _age = 0;
        private string _height = "";
        private string _weight = "";
        private string _gender = "";
        private string _ethnicity = "";
        private string _nationality = "";
        private string _deity = "";
        private string _class = "";
        private string _ancestry = "";
        private string _heritage = "";
        private string _background = "";
        private double _coin = 0;
        private readonly List<string> _languages = new();
        private readonly List<string> _edicts = new();
        private readonly List<string> _anathema = new();
        private readonly Dictionary<string, uint> _skills = new();
        private readonly Dictionary<string, uint> _lore = new();
        private readonly Dictionary<string, uint> _saves = new();
        private readonly List<string> _feats = new();
        private readonly List<string> _spells = new();

        private readonly List<string> TypeFilter =
            new()
            {
                "feat",
                "lore",
                "class",
                "ancestry",
                "heritage",
                "background",
                "spell",
                "treasure"
            };

        public Dictionary<string, dynamic>? GetCharacterData()
        {
            return null;
        }
    }
}
