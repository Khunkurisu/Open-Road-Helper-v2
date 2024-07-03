using Newtonsoft.Json;

namespace Bot.Quests
{
    public partial class Quest
    {
        [JsonConstructor]
        public Quest(
            Guid id,
            string name,
            ulong gameMaster,
            ulong guildId,
            string description,
            Threats threat,
            List<string> tags,
            int minPlayers,
            int maxPlayers,
            List<Guid> parties,
            Guid? selectedParty,
            Status status,
            ulong thread,
            ulong message,
            long createdAt,
            long lastUpdated
        )
        {
            _id = id;
            _name = name;
            _gameMaster = gameMaster;
            _guild = guildId;
            _description = description;
            _threat = threat;
            _tags = tags;
            _minPlayers = minPlayers;
            _maxPlayers = maxPlayers;
            _parties = parties;
            _selectedParty = selectedParty;
            _status = status;
            _thread = thread;
            _message = message;
            _createdAt = createdAt;
            _lastUpdated = lastUpdated;
        }
    }
}
