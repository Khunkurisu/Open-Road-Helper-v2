using System.Collections.ObjectModel;
using Bot.Characters;
using Bot.Helpers;
using Bot.Quests;
using Discord;
using Newtonsoft.Json;

namespace Bot.Guilds
{
    public class Guild
    {
        private readonly ulong _id;
        private const string _guildDataPath = @".\data\guilds\";
        private const string _availabilityPath = @"\availability.json";
        private const string _charactersPath = @"\characters.json";
        private const string _questsPath = @"\quests.json";
        private const string _partiesPath = @"\parties.json";
        private const string _boardsPath = @"\boards.json";

        private readonly List<Quest> _quests = new();
        private readonly List<Character> _characters = new();
        private readonly Dictionary<ulong, FoundryImport> _characterValues = new();
        private readonly Dictionary<string, Availability> _gmAvailabilities = new();
        private readonly List<Party> _parties = new();

        public IForumChannel? QuestBoard;
        public IForumChannel? TransactionBoard;
        public IForumChannel? CharacterBoard;

        public Guild(ulong id)
        {
            _id = id;
        }

        public void StoreTempCharacter(ulong playerId, FoundryImport foundryImport)
        {
            if (!_characterValues.ContainsKey(playerId))
            {
                _characterValues.Add(playerId, foundryImport);
            }
            else
            {
                _characterValues[playerId] = foundryImport;
            }
        }

        public FoundryImport? GetCharacterJson(ulong playerId)
        {
            return _characterValues.ContainsKey(playerId) ? _characterValues[playerId] : null;
        }

        public void LoadAll()
        {
            if (!Directory.Exists(_guildDataPath + _id))
            {
                Directory.CreateDirectory(_guildDataPath + _id);
            }
            LoadCharacters();
            LoadQuests();
            LoadAvailability();
            LoadParties();
            LoadBoards();
        }

        private void LoadAvailability()
        {
            if (!File.Exists(_guildDataPath + _id + _availabilityPath))
            {
                return;
            }
            string jsonString = File.ReadAllText(_guildDataPath + _id + _availabilityPath);
            Dictionary<string, Availability>? gmAvailabilities = JsonConvert.DeserializeObject<
                Dictionary<string, Availability>
            >(jsonString);
            if (gmAvailabilities != null)
            {
                foreach (string gm in gmAvailabilities.Keys)
                {
                    _gmAvailabilities.Add(gm, gmAvailabilities[gm]);
                }
            }
        }

        private void LoadCharacters()
        {
            if (!File.Exists(_guildDataPath + _id + _charactersPath))
            {
                return;
            }
            string jsonString = File.ReadAllText(_guildDataPath + _id + _charactersPath);
            List<Character>? characters = JsonConvert.DeserializeObject<List<Character>>(
                jsonString
            );
            if (characters != null)
            {
                foreach (Character character in characters)
                {
                    _characters.Add(character);
                }
            }
        }

        private void LoadBoards()
        {
            if (!File.Exists(_guildDataPath + _id + _boardsPath))
            {
                return;
            }
            string jsonString = File.ReadAllText(_guildDataPath + _id + _boardsPath);
            Dictionary<string, ulong>? boards = JsonConvert.DeserializeObject<
                Dictionary<string, ulong>
            >(jsonString);
            if (boards != null)
            {
                if (boards.ContainsKey("Characters"))
                {
                    CharacterBoard = BotManager.GetForumChannel(Id, boards["Characters"]);
                }
                if (boards.ContainsKey("Quests"))
                {
                    QuestBoard = BotManager.GetForumChannel(Id, boards["Quests"]);
                }
                if (boards.ContainsKey("Transactions"))
                {
                    TransactionBoard = BotManager.GetForumChannel(Id, boards["Transactions"]);
                }
            }
        }

        private void LoadQuests()
        {
            if (!File.Exists(_guildDataPath + _id + _questsPath))
            {
                return;
            }
            string jsonString = File.ReadAllText(_guildDataPath + _id + _questsPath);
            List<Quest>? quests = JsonConvert.DeserializeObject<List<Quest>>(jsonString);
            if (quests != null)
            {
                foreach (Quest quest in quests)
                {
                    _quests.Add(quest);
                }
            }
        }

        private void LoadParties()
        {
            if (!File.Exists(_guildDataPath + _id + _partiesPath))
            {
                return;
            }
            string jsonString = File.ReadAllText(_guildDataPath + _id + _partiesPath);
            List<Party>? parties = JsonConvert.DeserializeObject<List<Party>>(jsonString);
            if (parties != null)
            {
                foreach (Party party in parties)
                {
                    _parties.Add(party);
                }
            }
        }

        public void SaveAll()
        {
            if (!Directory.Exists(_guildDataPath + _id))
            {
                Directory.CreateDirectory(_guildDataPath + _id);
            }
            SaveCharacters();
            SaveQuests();
            SaveAvailability();
            SaveParties();
            SaveBoards();
        }

        private void SaveBoards()
        {
            Dictionary<string, ulong> boards = new();
            if (CharacterBoard != null)
            {
                boards.Add("Characters", CharacterBoard.Id);
            }
            if (QuestBoard != null)
            {
                boards.Add("Quests", QuestBoard.Id);
            }
            if (TransactionBoard != null)
            {
                boards.Add("Transactions", TransactionBoard.Id);
            }
            if (boards.Count > 0)
            {
                string json = JsonConvert.SerializeObject(boards);
                File.WriteAllText(_guildDataPath + _id + _boardsPath, json);
            }
        }

        private void SaveAvailability()
        {
            if (_gmAvailabilities.Count > 0)
            {
                string json = JsonConvert.SerializeObject(_gmAvailabilities);
                File.WriteAllText(_guildDataPath + _id + _availabilityPath, json);
            }
        }

        private void SaveCharacters()
        {
            if (_characters.Count > 0)
            {
                string json = JsonConvert.SerializeObject(_characters);
                File.WriteAllText(_guildDataPath + _id + _charactersPath, json);
            }
        }

        private void SaveQuests()
        {
            if (_quests.Count > 0)
            {
                string json = JsonConvert.SerializeObject(_quests);
                File.WriteAllText(_guildDataPath + _id + _questsPath, json);
            }
        }

        private void SaveParties()
        {
            if (_parties.Count > 0)
            {
                string json = JsonConvert.SerializeObject(_parties);
                File.WriteAllText(_guildDataPath + _id + _partiesPath, json);
            }
        }

        public void AddCharacter(Character character)
        {
            _characters.Add(character);
            SaveCharacters();
        }

        public void AddQuest(Quest quest)
        {
            _quests.Add(quest);
            SaveQuests();
        }

        public bool FormParty(Guid creator)
        {
            _parties.Add(new(creator));

            SaveParties();
            return true;
        }

        public bool FormParty(List<Character> members)
        {
            _parties.Add(new(members));

            SaveParties();
            return true;
        }

        public bool FormParty(Character creator)
        {
            _parties.Add(new(creator));

            SaveParties();
            return true;
        }

        public bool FormParty(List<Guid> members)
        {
            _parties.Add(new(members));

            SaveParties();
            return true;
        }

        public void RemoveCharacter(int index)
        {
            _characters.RemoveAt(index);
            SaveCharacters();
        }

        public void RemoveQuest(int index)
        {
            _quests.RemoveAt(index);
            SaveQuests();
        }

        public void RemoveQuest(Quest quest)
        {
            _quests.Remove(quest);
            SaveQuests();
        }

        public Quest? GetQuest(string name, IUser gm)
        {
            return Quests.First(x => x.Name == name && x.GameMaster == gm.Id);
        }

        public Quest? GetQuest(Guid id)
        {
            return Quests.First(x => x.Id == id);
        }

        public Character? GetCharacter(Guid characterId)
        {
            return _characters.First(x => x.Id == characterId);
        }

        public Party? GetParty(Guid partyId)
        {
            return _parties.First(x => x.Id == partyId);
        }

        public Availability GetAvailability(IUser gm)
        {
            if (!_gmAvailabilities.ContainsKey(gm.Username))
            {
                _gmAvailabilities.Add(gm.Username, new(gm));
            }

            SaveAvailability();
            return _gmAvailabilities[gm.Username];
        }

        public void AddAvailability(IUser gm, Timeframe newTimeframe)
        {
            if (_gmAvailabilities.ContainsKey(gm.Username))
            {
                Availability availability = _gmAvailabilities[gm.Username];
                availability.AddTimeframe(newTimeframe);
            }
            else
            {
                Availability gmAvailability = new(gm, newTimeframe);
                _gmAvailabilities.Add(gm.Username, gmAvailability);
            }
        }

        public void RemoveAvailability(IUser gm, Timeframe time)
        {
            if (_gmAvailabilities.ContainsKey(gm.Username))
            {
                Availability availability = _gmAvailabilities[gm.Username];
                availability.RemoveTimeframe(time);
            }
        }

        public void RemoveAvailability(IUser gm, int index)
        {
            if (_gmAvailabilities.ContainsKey(gm.Username))
            {
                Availability availability = _gmAvailabilities[gm.Username];
                availability.RemoveTimeframe(index);
            }
        }

        public ulong Id
        {
            get { return _id; }
        }
        public ReadOnlyCollection<Quest> Quests
        {
            get { return _quests.AsReadOnly(); }
        }
        public ReadOnlyCollection<Character> Characters
        {
            get { return _characters.AsReadOnly(); }
        }

        public ReadOnlyCollection<Party> Parties
        {
            get => _parties.AsReadOnly();
        }

        public ReadOnlyDictionary<string, Availability> Availability
        {
            get => _gmAvailabilities.AsReadOnly();
        }
    }
}
