using System.Collections.ObjectModel;
using System.Text.Json;
using Bot.Characters;
using Bot.Helpers;
using Bot.Quests;
using Discord;

namespace Bot.Guilds
{
    public readonly struct Guild
    {
        private readonly ulong _id;
        private const string _guildDataPath = @".\data\guilds\";
        private const string _availabilityPath = @"\availability.json";
        private const string _charactersPath = @"\characters.json";
        private const string _questsPath = @"\quests.json";
        private const string _partiesPath = @"\parties.json";
        private readonly List<Quest> _quests = new();
        private readonly List<Character> _characters = new();
        public readonly Dictionary<string, GMAvailability> GMAvailabilities = new();
        public readonly List<Party> Parties = new();

        public Guild(ulong id)
        {
            _id = id;
            LoadAll();
        }

        private void LoadAll()
        {
            LoadCharacters();
            LoadQuests();
            LoadAvailability();
            LoadParties();
        }

        private void LoadAvailability()
        {
            if (!File.Exists(_guildDataPath + _id + _availabilityPath))
            {
                return;
            }
            string json = File.ReadAllText(_guildDataPath + _id + _availabilityPath);
            if (
                JsonSerializer.Deserialize(json, GMAvailabilities.GetType())
                is Dictionary<string, GMAvailability> availability
            )
            {
                foreach (string gm in availability.Keys)
                {
                    GMAvailabilities[gm] = availability[gm];
                }
            }
        }

        private void LoadCharacters()
        {
            if (!File.Exists(_guildDataPath + _id + _charactersPath))
            {
                return;
            }
            string json = File.ReadAllText(_guildDataPath + _id + _charactersPath);
            if (
                JsonSerializer.Deserialize(json, _characters.GetType())
                is List<Character> characters
            )
            {
                foreach (Character character in characters)
                {
                    _characters.Add(character);
                }
            }
        }

        private void LoadQuests()
        {
            if (!File.Exists(_guildDataPath + _id + _questsPath))
            {
                return;
            }
            string json = File.ReadAllText(_guildDataPath + _id + _questsPath);
            if (JsonSerializer.Deserialize(json, _quests.GetType()) is List<Quest> quests)
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
            string json = File.ReadAllText(_guildDataPath + _id + _partiesPath);
            if (JsonSerializer.Deserialize(json, Parties.GetType()) is List<Party> parties)
            {
                foreach (Party party in parties)
                {
                    Parties.Add(party);
                }
            }
        }

        public void SaveAll()
        {
            SaveCharacters();
            SaveQuests();
            SaveAvailability();
            SaveParties();
        }

        private void SaveAvailability()
        {
            string json = JsonSerializer.Serialize(GMAvailabilities);
            File.WriteAllText(_guildDataPath + _id + _availabilityPath, json);
        }

        private void SaveCharacters()
        {
            string json = JsonSerializer.Serialize(Characters);
            File.WriteAllText(_guildDataPath + _id + _charactersPath, json);
        }

        private void SaveQuests()
        {
            string json = JsonSerializer.Serialize(Quests);
            File.WriteAllText(_guildDataPath + _id + _questsPath, json);
        }

        private void SaveParties()
        {
            string json = JsonSerializer.Serialize(Parties);
            File.WriteAllText(_guildDataPath + _id + _partiesPath, json);
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

        public bool FormParty(Character creator, Quest quest, DateTime startTime)
        {
            GMAvailability availability = GetAvailability(quest.GM);
            if (!availability.IsAvailable(startTime))
            {
                return false;
            }
            Parties.Add(new(creator, quest, startTime));

            SaveParties();
            return true;
        }

        public bool FormParty(List<Character> members, Quest quest, DateTime startTime)
        {
            GMAvailability availability = GetAvailability(quest.GM);
            if (!availability.IsAvailable(startTime))
            {
                return false;
            }
            Parties.Add(new(members, quest, startTime));

            SaveParties();
            return true;
        }

        public bool FormParty(Character creator, Quest quest, uint startTime)
        {
            GMAvailability availability = GetAvailability(quest.GM);
            if (!availability.IsAvailable(startTime))
            {
                return false;
            }
            Parties.Add(new(creator, quest, GenericHelpers.DateTimeFromUnixSeconds(startTime)));

            SaveParties();
            return true;
        }

        public bool FormParty(List<Character> members, Quest quest, uint startTime)
        {
            GMAvailability availability = GetAvailability(quest.GM);
            if (!availability.IsAvailable(startTime))
            {
                return false;
            }
            Parties.Add(new(members, quest, GenericHelpers.DateTimeFromUnixSeconds(startTime)));

            SaveParties();
            return true;
        }

        public void DisbandParty(Character creator, Quest quest)
        {
            foreach (Party party in Parties)
            {
                if (party.Members[1] == creator && party.Quest == quest)
                {
                    Parties.Remove(party);
                }
            }
            SaveParties();
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

        public GMAvailability GetAvailability(IUser gm)
        {
            if (!GMAvailabilities.ContainsKey(gm.Username))
            {
                GMAvailabilities.Add(gm.Username, new(gm));
            }

            SaveAvailability();
            return GMAvailabilities[gm.Username];
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
    }
}
