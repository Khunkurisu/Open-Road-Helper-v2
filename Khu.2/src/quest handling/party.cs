using System.Collections.ObjectModel;
using OpenRoadHelper.Characters;

namespace OpenRoadHelper.Quests
{
    public class Party
    {
        public Guid Id;
        public ulong Guild;
        public string Name;
        public List<string> MembersList;
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

        public void AddMember(string characterId)
        {
            MembersList.Add(characterId);
        }

        public void RemoveMember(string characterId)
        {
            MembersList.Remove(characterId);
        }

        public void RemoveMember(Character character)
        {
            MembersList.Remove(character.Id);
        }
    }
}
