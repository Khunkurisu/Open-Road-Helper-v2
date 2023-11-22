using static Bot.PF2.Helper;
namespace Bot.Characters {
	public class Skill {
		private string _name;
		private Proficiency _bonus;
		private Ability _attribute;
		private bool _isLore;

		public Skill (string name, Proficiency bonus, Ability attribute, bool isLore = false) {
			_name = name;
			_bonus = bonus;
			_attribute = attribute;
			_isLore = isLore;
		}

		public void UpdateProficiency (Proficiency bonus) {
			_bonus = bonus;
		}

		public string Name {
			get { return _name; }
		}
		public int Bonus {
			get { return (int) _bonus * 2; }
		}
		public Ability Attribute {
			get { return _attribute; }
		}
		public bool IsLore {
			get { return _isLore; }
		}
	}
}
