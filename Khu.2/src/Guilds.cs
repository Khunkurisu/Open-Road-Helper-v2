using System.Collections.ObjectModel;
using Bot.Characters;
using Bot.Quests;

namespace Bot.Guilds {
	public readonly struct Guild {
		private readonly ulong _id;
		private readonly List<Quest> _quests = new ();
		private readonly List<Character> _characters = new ();

		public Guild (ulong id) {
			_id = id;
		}

		public readonly void AddCharacter (Character character) {
			_characters.Add (character);
		}
		public readonly void AddQuest (Quest quest) {
			_quests.Add (quest);
		}
		public readonly void RemoveCharacter (int index) {
			_characters.RemoveAt (index);
		}
		public readonly void RemoveQuest (int index) {
			_quests.RemoveAt (index);
		}

		public readonly ulong Id {
			get { return _id; }
		}
		public readonly ReadOnlyCollection<Quest> Quests {
			get { return _quests.AsReadOnly (); }
		}
		public readonly ReadOnlyCollection<Character> Characters {
			get { return _characters.AsReadOnly (); }
		}
	}
}
