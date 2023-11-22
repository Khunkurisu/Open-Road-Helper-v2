using Bot.PF2;

namespace Bot.Characters {
	public class Ancestry : CharacterFeatures {
		private bool _restricted = false;
		private List<Ability> _attributes;
		private bool _altBoosts;

		public Ancestry (string name, Helper.Rarity rarity, List<string> traits, bool restricted, List<Ability> attributes, bool altBoosts) : base (name, rarity, traits) {
			_restricted = restricted;
			_attributes = attributes;
			_altBoosts = altBoosts;
		}

		public bool IsRestricted {
			get { return _restricted; }
		}
	}
}
