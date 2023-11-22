namespace Bot.PF2 {
	public readonly struct DiceRoll {
		public DiceRoll (short sides = 20, int count = 1) {
			for (int i = 0; i < count; i++) {
				int roll = rng.Next (1, sides + 1);
				_total += roll;
				_results.Add (roll);
			}
		}

		private readonly Random rng = new Random ();

		private readonly int _total = 0;
		private readonly List<int> _results = new ();

		public readonly int Total {
			get { return _total; }
		}
		public readonly List<int> Results {
			get { return _results; }
		}
	}

	public readonly struct CheckResult {
		public CheckResult (short roll, short modifier, short difficulty) {
			int result = (int) Helper.SuccessIndex.Fail;
			if (roll + modifier >= difficulty) {
				result = (int) Helper.SuccessIndex.Success;
			}
			if (roll == 20 || roll + modifier >= difficulty + 10) {
				result++;
			}
			if (roll == 1 || roll + modifier <= difficulty - 10) {
				result--;
			}
			result = Math.Clamp (result, 0, 3);
			_result = result;
		}

		private readonly int _result;
		public readonly int SuccessIndex {
			get { return _result; }
		}
		public readonly string DegreeOfSuccess {
			get { return Helper.DegreeOfSuccess[_result]; }
		}
	}
}
