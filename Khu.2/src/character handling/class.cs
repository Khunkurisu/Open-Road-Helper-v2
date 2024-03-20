using Bot.PF2;

namespace Bot.Characters
{
    public class Class : CharacterFeatures
    {
        public List<Skill> _skills;
        public List<Ability> _attributes;

        public Class(
            string name,
            Helper.Rarity rarity,
            List<string> traits,
            List<Skill> skills,
            List<Ability> attributes
        ) : base(name, rarity, traits)
        {
            _skills = skills;
            _attributes = attributes;
        }
    }
}
