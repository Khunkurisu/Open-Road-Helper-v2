using PF2;

namespace Characters {
	public class Class : CharacterFeatures {
		private List<Skill> _skills;
		private List<Ability> _attributes;

		public Class (string name, Helper.Rarity rarity, List<string> traits, List<Skill> skills, List<Ability> attributes) : base (name, rarity, traits) {
			_skills = skills;
			_attributes = attributes;
		}

		public List<Skill> Skills {
			get { return _skills; }
		}
		public List<Ability> KeyAttributes {
			get { return _attributes; }
		}
	}
}
