using Bot.PF2;

namespace Bot.Characters {
	public class Ancestry : CharacterFeatures {
		private bool _restricted = false;
		private List<Ability> _attributes;

		public Ancestry (string name, Helper.Rarity rarity, List<string> traits, List<Ability> attributes, bool restricted = false) : base (name, rarity, traits) {
			_restricted = restricted;
			_attributes = attributes;
		}

		public bool IsRestricted {
			get { return _restricted; }
		}
	}
}
