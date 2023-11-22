namespace Bot.Characters {
	public class Generation {
		public static int Total = 0;
		private readonly int _id;
		private List<Character> _characters = new ();
		private int _level;
		private long _created;

		public Generation (int level) {
			Total++;
			_id = Total;
			_level = level;
			_created = DateTimeOffset.UtcNow.ToUnixTimeSeconds ();
		}

		public int Level {
			get { return _level; }
		}
		public int Id {
			get { return _id; }
		}
		public List<Character> Characters {
			get { return _characters; }
		}
		public long CreatedOn {
			get { return _created; }
		}
	}
}
