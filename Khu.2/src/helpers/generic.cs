namespace Helpers {
	public static class GenericHelpers {
		public static string Ordinal (float num) {
			float ones = num % 10;
			float tens = MathF.Floor (num / 10) % 10;
			string suffix;
			if (tens == 1) {
				suffix = "th";
			} else {
				switch (ones) {
					case 1:
						suffix = "st";
						break;
					case 2:
						suffix = "nd";
						break;
					case 3:
						suffix = "rd";
						break;
					default:
						suffix = "th";
						break;
				}
			}
			return $"{num}{suffix}";
		}
	}
}
