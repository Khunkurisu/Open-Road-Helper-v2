namespace Bot.Characters
{
    public partial class Character
    {
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
    }
}
