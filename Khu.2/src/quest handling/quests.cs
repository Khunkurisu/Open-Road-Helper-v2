using Bot.Characters;
using Discord;

namespace Bot.Quests {
	public class Quest {
		public static int Total = 0;
		private readonly int _id;
		private readonly string _name;
		private readonly IUser _gm;
		private string _desc;
		private int _threat;
		private List<string> _tags;
		private long _time;
		private int _level;
		private int _maxPlayers;
		private List<Character> _chars = new ();
		private int _status = 0;
		private ulong _messageId;
		private ulong _threadId;
		private readonly long _created;

		public Quest (string name, IUser gm, string desc, int threat, List<string> tags, long time, int level, int maxPlayers,
			ulong messageId, ulong threadId) {
			Total++;
			_id = Total;
			_name = name;
			_gm = gm;
			_desc = desc;
			_threat = threat;
			_tags = tags;
			_time = time;
			_level = level;
			_maxPlayers = maxPlayers;
			_messageId = messageId;
			_threadId = threadId;
			_created = DateTimeOffset.UtcNow.ToUnixTimeSeconds ();
		}
		public void SetStatus (Status status) {
			_status = (int) status;
		}

		public int Id {
			get { return _id; }
		}
		public string Name {
			get { return _name; }
		}
		public IUser GM {
			get { return _gm; }
		}
		public string Desc {
			get { return _desc; }
			set { _desc = value; }
		}
		public int Threat {
			get { return _threat; }
			set { _threat = value; }
		}
		public List<string> Tags {
			get { return _tags; }
		}
		public long Time {
			get { return _time; }
			set { _time = value; }
		}
		public int Level {
			get { return _level; }
			set { _level = value; }
		}
		public int MaxPlayers {
			get { return _maxPlayers; }
			set { _maxPlayers = value; }
		}
		public List<Character> Characters {
			get { return _chars; }
		}
		public int Status {
			get { return _status; }
		}
		public ulong MessageId {
			get { return _messageId; }
			set { _messageId = value; }
		}
		public ulong ThreadId {
			get { return _threadId; }
			set { _threadId = value; }
		}
		public long Created {
			get { return _created; }
		}
	}
}
