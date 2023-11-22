namespace Bot.PF2 {
	public static class Helper {
		public static int[] LevelBasedDCs = new int[] {
			14,
			15,
			16,
			18,
			19,
			20,
			22,
			23,
			24,
			26,
			27,
			28,
			30,
			31,
			32,
			34,
			35,
			36,
			38,
			39,
			40,
			42,
			44,
			46,
			48,
			50
		};
		public static int[] SimpleDCs = new int[] {
			10,
			15,
			20,
			30,
			40
		};
		public static int[] DCAdjustment = new int[] {
			-10, -5, -2, 2, 5, 10
		};
		public static double[, ] IncomeTable = new double[, ] { { 14, 0.01, 0.05, 0.05, 0.05, 0.05 }, { 15, 0.02, 0.2, 0.2, 0.2, 0.2 }, { 16, 0.04, 0.3, 0.3, 0.3, 0.3 }, { 18, 0.08, 0.5, 0.5, 0.5, 0.5 }, { 19, 0.1, 0.7, 0.8, 0.8, 0.8 }, { 20, 0.2, 0.9, 1, 1, 1 }, { 22, 0.3, 1.5, 2, 2, 2 }, { 23, 0.4, 2, 2.5, 2.5, 2.5 }, { 24, 0.5, 2.5, 3, 3, 3 }, { 26, 0.6, 3, 4, 4, 4 }, { 27, 0.7, 4, 5, 6, 6 }, { 28, 0.8, 5, 6, 8, 8 }, { 30, 0.9, 6, 8, 10, 10 }, { 31, 1, 7, 10, 15, 15 }, { 32, 1.5, 8, 15, 20, 20 }, { 34, 2, 10, 20, 28, 28 }, { 35, 2.5, 13, 25, 36, 40 }, { 36, 3, 15, 30, 45, 55 }, { 38, 4, 20, 45, 70, 90 }, { 39, 6, 30, 60, 100, 130 }, { 40, 8, 40, 75, 150, 200 }, { 40, 8, 50, 90, 175, 300 }
		};
		public static string[] DegreeOfSuccess = new string[] {
			"critical failure",
			"failure",
			"success",
			"critical success"
		};
		public enum SuccessIndex {
			CritFail,
			Fail,
			Success,
			CritSuccess
		}
		public enum Proficiency {
			Untrained,
			Trained,
			Expert,
			Master,
			Legendary
		}

		public static int[, ] CharacterWealth = new int[, ] {
			{
				15,
				15
			}, {
				20,
				30
			}, {
				25,
				75
			}, {
				30,
				140
			}, {
				50,
				270
			}, {
				80,
				450
			}, {
				125,
				720
			}, {
				180,
				1100
			}, {
				250,
				1600
			}, {
				350,
				2300
			}, {
				500,
				3200
			}, {
				700,
				4500
			}, {
				1000,
				6400
			}, {
				1500,
				9300
			}, {
				2250,
				13500
			}, {
				3250,
				20000
			}, {
				5000,
				30000
			}, {
				7500,
				45000
			}, {
				12000,
				69000
			}, {
				20000,
				112000
			}
		};
		public static int[, ] PartyTreasure = new int[, ] { { 40, 10 }, { 70, 18 }, { 120, 30 }, { 200, 50 }, { 320, 80 }, { 500, 125 }, { 720, 180 }, { 1000, 250 }, { 1400, 350 }, { 2000, 500 }, { 2800, 700 }, { 4000, 1000 }, { 6000, 1500 }, { 9000, 2250 }, { 13000, 3250 }, { 20000, 5000 }, { 30000, 7500 }, { 48000, 12000 }, { 80000, 20000 }, { 140000, 35000 },
		};
		public static float[] ThreatMults = new float[] {
			0.5f,
			0.75f,
			1.0f,
			1.5f,
			2.0f
		};
		public enum Threats {
			Trivial,
			Low,
			Moderate,
			Severe,
			Extreme
		}

		public enum Rarity {
			Common,
			Uncommon,
			Rare,
			Unique
		}

		public static float StartingGold (int level) {
			float gold = CharacterWealth[level - 1, 0];
			if (level > 3) {
				gold *= 3.0f;
			}
			return gold;
		}

		public static float QuestReward (int level, Threats threat) {
			float gold = PartyTreasure[level - 1, 0] / 3.0f;
			float multiplier = ThreatMults[(int) threat];
			return gold * multiplier;
		}
	}
}
