using System.Collections.ObjectModel;
using Bot.Characters;

namespace Bot.Quests
{
    public class Party
    {
        public Guid Id;
        public ulong Guild;
        public string Name;
        public List<Guid> MembersList;
        public string Members => string.Join(", ", MembersList);

        public Party(Character creator)
        {
            Name = creator.Name + "'s Party";
            MembersList = new() { creator.Id };
        }

        public Party(List<Character> members)
        {
            Name = members[0].Name + "'s Party";
            MembersList = new();
            foreach (Character character in members)
            {
                MembersList.Add(character.Id);
            }
        }

        public void AddMember(Character character)
        {
            MembersList.Add(character.Id);
        }

        public void AddMember(Guid characterId)
        {
            MembersList.Add(characterId);
        }

        public void RemoveMember(Guid characterId)
        {
            MembersList.Remove(characterId);
        }

        public void RemoveMember(Character character)
        {
            MembersList.Remove(character.Id);
        }
    }
}
