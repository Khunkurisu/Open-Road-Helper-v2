using Newtonsoft.Json;

namespace Bot.Characters
{
    public partial class Character
    {
        [JsonConstructor]
        public Character(
            Guid id,
            ulong user,
            ulong guild,
            ulong thread,
            string? name,
            string? desc,
            string? rep,
            uint age,
            string? deity,
            string? gender,
            float height,
            float weight,
            int birthDay,
            Months birthMonth,
            int birthYear,
            string? ancestry,
            string? heritage,
            string? charClass,
            string? background,
            uint level,
            int generation,
            double currency,
            int downtime,
            List<string> languages,
            Dictionary<string, uint> skills,
            Dictionary<string, uint> lore,
            Dictionary<string, uint> saves,
            uint perception,
            List<string> feats,
            List<string> spells,
            List<string> edicts,
            List<string> anathema,
            Dictionary<string, uint> attributes,
            string colorPref,
            List<string> avatars,
            List<string> notes,
            int lastTokenTrade,
            long created,
            long updated,
            Status status,
            Display display
        )
        {
            _id = id;
            _user = user;
            _guild = guild;
            _thread = thread;
            _name = name;
            _desc = desc;
            _rep = rep;
            _age = age;
            _deity = deity;
            _gender = gender;
            _height = height;
            _weight = weight;
            _birthDay = birthDay;
            _birthMonth = birthMonth;
            _birthYear = birthYear;
            _ancestry = ancestry;
            _heritage = heritage;
            _class = charClass;
            _background = background;
            _level = level;
            _generation = generation;
            _currency = currency;
            _downtime = downtime;
            _languages = languages;
            _skills = skills;
            _lore = lore;
            _saves = saves;
            _perception = perception;
            _feats = feats;
            _spells = spells;
            _edicts = edicts;
            _anathema = anathema;
            _attributes = attributes;
            _colorPref = colorPref;
            _avatars = avatars;
            _notes = notes;
            _lastTokenTrade = lastTokenTrade;
            _created = created;
            _updated = updated;
            _status = status;
            _display = display;
        }
    }
}
