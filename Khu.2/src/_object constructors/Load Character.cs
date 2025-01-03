using Newtonsoft.Json;

namespace OpenRoadHelper.Characters
{
    public partial class Character
    {
        [JsonConstructor]
        public Character(
            string id,
            ulong user,
            ulong guild,
            ulong charThread,
            ulong transThread,
            string? name,
            string? desc,
            string? rep,
            uint age,
            string? deity,
            string? gender,
            float height,
            float weight,
            int birthDay,
            PF2E.Months birthMonth,
            int birthYear,
            string? ancestry,
            string? heritage,
            string? Class,
            string? background,
            uint level,
            int generation,
            double currency,
            uint downtime,
            List<string> languages,
            Dictionary<string, int> skills,
            Dictionary<string, int> lore,
            Dictionary<string, int> saves,
            uint perception,
            List<string> feats,
            List<string> spells,
            List<string> edicts,
            List<string> anathema,
            Dictionary<string, uint> attributes,
            string colorPref,
            Dictionary<string, string> avatars,
            int avatar,
            List<string> notes,
            int lastTokenTrade,
            long created,
            long updated,
            Status status,
            Display display,
            ImportType? importType,
            float? saveVersion
        )
        {
            if (id.Length > 22)
            {
                _id = new(id);
            }
            else
            {
                _id = new(Convert.FromBase64String(id + "=="));
            }
            _user = user;
            _guild = guild;
            _characterThread = charThread;
            _transactionThread = transThread;
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
            _class = Class;
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
            _currentAvatar = avatar;
            _importType = importType ?? ImportType.Foundry;
            saveVersion ??= 0.0f;
            if (saveVersion < _saveVersion)
            {
                MigrateSave((float)saveVersion);
            }
        }

        private void MigrateSave(float saveVersion)
        {
            if (saveVersion < 0.1 && ImportType == ImportType.Pathbuilder)
            {
                foreach (string save in _saves.Keys)
                {
                    _saves[save] = PF2E.SaveBonus(save, (int)_level, _feats, _attributes, _saves);
                }
                foreach (string lore in _lore.Keys)
                {
                    _lore[lore] = PF2E.SkillBonus(lore, (int)_level, _feats, _attributes, _lore);
                }
                _perception = (uint)
                    PF2E.PerceptionBonus((int)_perception, (int)_level, _feats, _attributes);
            }
            Manager.GetGuild(_guild).QueueSave(SaveType.Characters, true);
        }
    }
}
