namespace Bot.Quests
{
    public partial class Quest
    {
        private readonly Guid _id;
        private readonly string _name = "temp";
        private readonly ulong _gameMaster;
        private string _description = "temp";
        private Threats _threat;
        private List<string> _tags = new();
        private int _minPlayers;
        private int _maxPlayers;
        private List<Guid> _parties = new();
        private Guid? _selectedParty;
        private Status _status = 0;
        private ulong _thread;
        private readonly long _createdAt;
        private long _lastUpdated;
    }
}