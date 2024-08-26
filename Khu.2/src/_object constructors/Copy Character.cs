namespace OpenRoadHelper.Characters
{
    public partial class Character
    {
        public Character(Character template)
        {
            _id = template._id;
            _user = template._user;
            _guild = template._guild;
            _characterThread = template._characterThread;
            _transactionThread = template._transactionThread;
            _name = template._name;
            _desc = template._desc;
            _rep = template._rep;
            _age = template._age;
            _deity = template._deity;
            _gender = template._gender;
            _height = template._height;
            _weight = template._weight;
            _birthDay = template._birthDay;
            _birthMonth = template._birthMonth;
            _birthYear = template._birthYear;
            _ancestry = template._ancestry;
            _heritage = template._heritage;
            _class = template._class;
            _background = template._background;
            _level = template._level;
            _generation = template._generation;
            _currency = template._currency;
            _downtime = template._downtime;
            _languages = template._languages;
            _skills = template._skills;
            _lore = template._lore;
            _saves = template._saves;
            _perception = template._perception;
            _feats = template._feats;
            _spells = template._spells;
            _edicts = template._edicts;
            _anathema = template._anathema;
            _attributes = template._attributes;
            _colorPref = template._colorPref;
            _avatars = template._avatars;
            _notes = template._notes;
            _lastTokenTrade = template._lastTokenTrade;
            _created = template._created;
            _updated = template._updated;
            _status = template._status;
            _display = template._display;
            _currentAvatar = template._currentAvatar;
        }
    }
}
