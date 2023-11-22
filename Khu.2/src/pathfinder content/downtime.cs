namespace PF2 {
	using static Helper;
	public abstract class Downtime {
		protected readonly short _modifier;
		protected readonly Proficiency _proficiency;
		protected readonly short _level;
		public abstract void Perform ();
		protected CheckResult result;
		protected DiceRoll roll;

		public Downtime (short modifier, short level, Proficiency proficiency) {
			_modifier = modifier;
			_proficiency = proficiency;
			_level = level;
		}

		protected short[, ] SetupTime = new short[, ] { { 4, 6 }, { 3, 5 }, { 3, 5 }, { 2, 4 } };
		protected double[, ] IncomeTable = new double[, ] { { 14, 0.01, 0.05, 0.05, 0.05, 0.05 }, { 15, 0.02, 0.2, 0.2, 0.2, 0.2 }, { 16, 0.04, 0.3, 0.3, 0.3, 0.3 }, { 18, 0.08, 0.5, 0.5, 0.5, 0.5 }, { 19, 0.1, 0.7, 0.8, 0.8, 0.8 }, { 20, 0.2, 0.9, 1, 1, 1 }, { 22, 0.3, 1.5, 2, 2, 2 }, { 23, 0.4, 2, 2.5, 2.5, 2.5 }, { 24, 0.5, 2.5, 3, 3, 3 }, { 26, 0.6, 3, 4, 4, 4 }, { 27, 0.7, 4, 5, 6, 6 }, { 28, 0.8, 5, 6, 8, 8 }, { 30, 0.9, 6, 8, 10, 10 }, { 31, 1, 7, 10, 15, 15 }, { 32, 1.5, 8, 15, 20, 20 }, { 34, 2, 10, 20, 28, 28 }, { 35, 2.5, 13, 25, 36, 40 }, { 36, 3, 15, 30, 45, 55 }, { 38, 4, 20, 45, 70, 90 }, { 39, 6, 30, 60, 100, 130 }, { 40, 8, 40, 75, 150, 200 }, { 40, 8, 50, 90, 175, 300 }
		};
	}

	public class Crafting : Downtime {
		private readonly short _setup;
		private readonly short _difficulty;
		private int _daysSpent;
		private readonly short _maxDays;
		private readonly float _cost;
		private float _reducedBy;

		public Crafting (short modifier, short charLevel, short itemLevel, Rarity rarity, bool consumable,
			double rushed, short maxDays, Proficiency proficiency, float cost) : base (modifier, charLevel, proficiency) {
			_maxDays = maxDays;
			_cost = cost;
			int level = Math.Clamp (charLevel - itemLevel, 0, 3);
			roll = new ();
			int adjustment = 0;
			if (rarity != Rarity.Common) {
				adjustment = DCAdjustment[(int) rarity + 2];
			}
			_difficulty = (short) (LevelBasedDCs[itemLevel] + adjustment + rushed * 5);
			_setup = SetupTime[level, consumable ? 1 : 0];
			Perform ();
		}

		public override void Perform () {
			result = new ((short) roll.Total, _modifier, _difficulty);
			double reduction = IncomeTable[_level, (int) _proficiency];

			_daysSpent = (int) Math.Min (Math.Ceiling (_cost / 2 / reduction), _maxDays);
			float reducedBy = _daysSpent * (float) reduction;

			_reducedBy = Math.Min (reducedBy, _cost / 2.0f);
		}

		public int SuccessIndex {
			get { return result.SuccessIndex; }
		}
		public string DegreeOfSuccess {
			get { return result.DegreeOfSuccess; }
		}
		public int DaysSpent {
			get { return _daysSpent + _setup; }
		}
		public float ReducedBy {
			get { return _reducedBy; }
		}
		public float FinalCost {
			get { return _cost - _reducedBy; }
		}
	}

	public class EarnIncome : Downtime {
		private readonly short _difficulty;
		private int _daysSpent;
		private float _earned = 0;

		public EarnIncome (int daysSpent, short modifier, Proficiency proficiency, short level) : base (modifier, level, proficiency) {
			_daysSpent = daysSpent;
			_difficulty = (short) Helper.IncomeTable[_level, 0];
			roll = new ();
			Perform ();
		}

		public override void Perform () {
			int prof = 1;
			result = new ((short) roll.Total, _modifier, _difficulty);
			if (roll.Total + _modifier >= _difficulty) {
				prof = (int) _proficiency + 1;
			}
			int level = _level;
			if (result.SuccessIndex == (int) Helper.SuccessIndex.CritSuccess) {
				level++;
			}
			level = Math.Clamp (level, 0, 21);

			_earned = (float) IncomeTable[_level, (int) _proficiency] * 3.0f;
			_earned *= _daysSpent;
			if (result.SuccessIndex == (int) Helper.SuccessIndex.CritFail) {
				_earned = 0;
				_daysSpent = 0;
			}
		}

		public float Earned {
			get { return _earned; }
		}
		public int SuccessIndex {
			get { return result.SuccessIndex; }
		}
		public string DegreeOfSuccess {
			get { return result.DegreeOfSuccess; }
		}
		public int DaysSpent {
			get { return _daysSpent; }
		}
	}
}
