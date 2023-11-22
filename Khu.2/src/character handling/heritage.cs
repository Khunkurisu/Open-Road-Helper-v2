using Bot.PF2;

namespace Bot.Characters {
	public class Heritage : CharacterFeatures {
		private bool _versatile = false;

		public Heritage (string name, Helper.Rarity rarity, List<string> traits, bool versatile) : base (name, rarity, traits) {
			_versatile = versatile;
		}

		public bool IsCompatibleAncestry (Ancestry ancestry) {
			bool compat = _versatile;
			if (!compat) {
				for (int i = 0; i < _traits.Count; i++) {
					if (ancestry.HasTrait (_traits[i])) {
						compat = true;
						break;
					}
				}
			}
			return compat;
		}

		public bool IsVersatile {
			get { return _versatile; }
		}

	}
}
