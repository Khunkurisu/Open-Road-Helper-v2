using Newtonsoft.Json;

namespace OpenRoadHelper.Quests
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
            dynamic loadedTags,
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
            if (loadedTags is string allTags)
            {
                _tags = new(
                    allTags.Split(
                        ",",
                        StringSplitOptions.TrimEntries & StringSplitOptions.RemoveEmptyEntries
                    )
                );
            }
            else
            {
                _tags = loadedTags;
            }
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
