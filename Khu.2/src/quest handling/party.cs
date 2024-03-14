using System.Collections.ObjectModel;
using Bot.Characters;

namespace Bot.Quests
{
    public class Party
    {
        private readonly List<Character> _members;
        private readonly Quest _quest;
        private readonly DateTime _chosenTime;

        public Party(Character creator, Quest quest, DateTime chosenTime)
        {
            _members = new() { creator };
            _quest = quest;
            _chosenTime = chosenTime;
        }

        public Party(List<Character> members, Quest quest, DateTime chosenTime)
        {
            _members = members;
            _quest = quest;
            _chosenTime = chosenTime;
        }

        public ReadOnlyCollection<Character> Members
        {
            get => _members.AsReadOnly();
        }

        public Quest Quest
        {
            get => _quest;
        }

        public DateTime ChosenTime
        {
            get => _chosenTime;
        }

        public void AddMember(Character character)
        {
            if (_members.Count < _quest.MaxPlayers)
            {
                _members.Add(character);
            }
        }

        public void RemoveMember(Character character)
        {
            _members.Remove(character);
        }
    }
}
