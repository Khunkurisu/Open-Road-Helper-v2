namespace OpenRoadHelper.Quests
{
    public partial class Quest
    {
        public Quest(Quest template)
        {
            _id = template._id;
            _name = template._name;
            _gameMaster = template._gameMaster;
            _guild = template._guild;
            _description = template._description;
            _threat = template._threat;
            _tags = template._tags;
            _minPlayers = template._minPlayers;
            _maxPlayers = template._maxPlayers;
            _parties = template._parties;
            _selectedParty = template._selectedParty;
            _status = template._status;
            _thread = template._thread;
            _message = template._message;
            _createdAt = template._createdAt;
            _lastUpdated = template._lastUpdated;
        }
    }
}
