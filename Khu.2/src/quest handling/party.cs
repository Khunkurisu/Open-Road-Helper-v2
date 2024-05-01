using System.Collections.ObjectModel;
using Bot.Characters;

namespace Bot.Quests
{
    public class Party
    {
        public Guid Id;
        public ulong Guild;
        public string Name;
        public List<Guid> Members;

        public Party(Character creator)
        {
            Name = creator.Name + "'s Party";
            Members = new() { creator.Id };
        }

        public Party(Guid creatorId)
        {
            Character creator = BotManager.GetCharacter(Guild, creatorId);
            Name = creator.Name + "'s Party";
            Members = new() { creator.Id };
        }

        public Party(List<Guid> memberIds)
        {
            Character creator = BotManager.GetCharacter(Guild, memberIds[0]);
            Name = creator.Name + "'s Party";
            Members = memberIds;
        }

        public Party(List<Character> members)
        {
            Name = members[0].Name + "'s Party";
            Members = new();
            foreach (Character character in members)
            {
                Members.Add(character.Id);
            }
        }

        public void AddMember(Character character)
        {
            Members.Add(character.Id);
        }

        public void AddMember(Guid characterId)
        {
            Members.Add(characterId);
        }

        public void RemoveMember(Guid characterId)
        {
            Members.Remove(characterId);
        }

        public void RemoveMember(Character character)
        {
            Members.Remove(character.Id);
        }
    }
}
