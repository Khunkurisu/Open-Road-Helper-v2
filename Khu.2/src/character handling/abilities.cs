namespace Characters {
	public class Ability {
		private string _name;
		private short _modifier = 0;
		private bool _partial = false;
		public Ability (string name, short modifier = 0) {
			_name = name;
			_modifier = modifier;
		}

		public void AddBoost () {
			if (_modifier < 4 || _partial) {
				_modifier++;
				_partial = false;
			} else {
				_partial = true;
			}
		}

		public string Name {
			get { return _name; }
		}
		public short Modifier {
			get { return _modifier; }
		}
		public bool IsPartial {
			get { return _partial; }
		}
	}
}
