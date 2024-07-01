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
            string name,
            string desc,
            string rep,
            uint age,
            float height,
            float weight,
            int birthday,
            Months birthMonth,
            int birthYear,
            string ancestry,
            string heritage,
            string charClass,
            string background,
            int generation,
            uint level,
            double gold,
            int downtime,
            string deity,
            string gender,
            string color,
            string avatarURL,
            ulong messageId,
            List<string> notes,
            int lastTokenTrade,
            long createdOn,
            long updatedOn,
            Status status,
            Display display,
            List<string> feats,
            List<string> spells,
            List<string> edicts,
            List<string> anathema,
            List<string> languages,
            Dictionary<string, uint> skills,
            Dictionary<string, uint> lore,
            Dictionary<string, uint> saves,
            uint perception,
            Dictionary<string, uint> attributes
        )
        {
            _id = id;
            _user = user;
            _guild = guild;
            _name = name;
            _desc = desc;
            _rep = rep;
            _age = age;
            _height = height;
            _weight = weight;
            _birthDay = birthday;
            _birthMonth = birthMonth;
            _birthYear = birthYear;
            _ancestry = ancestry;
            _heritage = heritage;
            _class = charClass;
            _background = background;
            _generation = generation;
            _level = level;
            _currency = gold;
            _downtime = downtime;
            _colorPref = color;
            _avatars[0] = avatarURL;
            _notes = notes;
            _lastTokenTrade = lastTokenTrade;
            _created = createdOn;
            _updated = updatedOn;
            _status = status;
            _display = display;
            _thread = thread;
            _feats = feats;
            _spells = spells;
            _edicts = edicts;
            _anathema = anathema;
            _languages = languages;
            _skills = skills;
            _lore = lore;
            _saves = saves;
            _perception = perception;
            _attributes = attributes;
            _deity = deity;
            _gender = gender;
        }
    }
}
