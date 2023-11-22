using Bot.PF2;
namespace Bot.Characters {
	public abstract class CharacterFeatures {
		protected readonly string _name;
		protected int _rarity = 0;
		protected List<string> _traits;
		public CharacterFeatures (string name, Helper.Rarity rarity, List<string> traits) {
			_name = name;
			_rarity = (int) rarity;
			_traits = traits;
		}
		public bool HasTrait (string trait) {
			bool has = false;
			for (int i = 0; i < _traits.Count; i++) {
				if (_traits[i] == trait) {
					has = true;
					break;
				}
			}
			return has;
		}

		public string Name {
			get { return _name; }
		}
		public int Rarity {
			get { return _rarity; }
		}
		public List<string> Traits {
			get { return _traits; }
		}
	}
}
