using Bot.Helpers;

namespace Bot.Characters
{
    public class FoundryImport : IImportable
    {
        public FoundryImport(Dictionary<string, dynamic> jsonData)
        {
            jsonData.Remove("prototypeToken");
            jsonData.Remove("type");
            jsonData.Remove("effects");
            jsonData.Remove("img");
            jsonData.Remove("folder");
            jsonData.Remove("flags");
            jsonData.Remove("_stats");

            _name = jsonData["name"];

            Dictionary<string, dynamic> systemData = jsonData["system"].ToObject<
                Dictionary<string, dynamic>
            >();

            systemData.Remove("attributes");
            systemData.Remove("initiative");
            systemData.Remove("resources");
            systemData.Remove("_migration");
            systemData.Remove("abilities");
            systemData.Remove("pfs");
            systemData.Remove("exploration");

            Console.WriteLine("trying to collate system data");

            CollateSystemData(systemData);

            Console.WriteLine("finished trying to collate system data");

            List<Dictionary<string, dynamic>> itemData = jsonData["items"].ToObject<
                List<Dictionary<string, dynamic>>
            >();
            List<Dictionary<string, dynamic>> toRemove = new();
            foreach (Dictionary<string, dynamic> dict in itemData)
            {
                if (dict.ContainsKey("type"))
                {
                    string type = dict["type"];
                    if (!TypeFilter.Contains(type))
                    {
                        toRemove.Add(dict);
                    }
                }
            }
            foreach (Dictionary<string, dynamic> dict in toRemove)
            {
                itemData.Remove(dict);
            }

            Console.WriteLine("trying to list items");

            try
            {
                foreach (Dictionary<string, dynamic> dict in itemData)
                {
                    if (dict.ContainsKey("name"))
                    {
                        Console.WriteLine(dict["name"]);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            //CollateItemData(itemData);
        }

        private void CollateSystemData(Dictionary<string, dynamic> systemData)
        {
            Dictionary<string, dynamic> details = systemData["details"].ToObject<
                Dictionary<string, dynamic>
            >();

            Dictionary<string, dynamic> biography = details["biography"].ToObject<
                Dictionary<string, dynamic>
            >();
            _description = biography["appearance"];
            _birthplace = biography["birthPlace"];
            _backstory = biography["backstory"];
            foreach (string edict in biography["edicts"])
            {
                _edicts.Add(edict);
            }
            foreach (string anathema in biography["anathema"])
            {
                _anathema.Add(anathema);
            }

            Dictionary<string, dynamic> languages = details["languages"].ToObject<
                Dictionary<string, dynamic>
            >();
            foreach (string language in languages["value"])
            {
                _languages.Add(language);
            }

            _age = details["age"]["value"];
            _height = details["height"]["value"];
            _weight = details["weight"]["value"];
            _gender = details["gender"]["value"];
            _ethnicity = details["ethnicity"]["value"];
            _nationality = details["nationality"]["value"];
            _deity = details["deity"]["value"];

            Dictionary<string, Dictionary<string, uint>> skills = systemData["skills"].ToObject<
                Dictionary<string, Dictionary<string, uint>>
            >();
            foreach (string skill in skills.Keys)
            {
                uint skillRank = skills[skill]["rank"];
                string skillName = GenericHelpers.ExpandSkillAbbreviation(skill);
                _skills.Add(skillName, skillRank);
            }

            Dictionary<string, Dictionary<string, uint>> saves = systemData["saves"].ToObject<
                Dictionary<string, Dictionary<string, uint>>
            >();
            foreach (string save in saves.Keys)
            {
                Dictionary<string, uint> saveHolder = saves[save];
                string valueKey = "value";
                if (!saveHolder.ContainsKey("value"))
                {
                    if (saveHolder.ContainsKey("rank"))
                    {
                        valueKey = "rank";
                    }
                    else
                    {
                        valueKey = "tank";
                    }
                }
                uint saveRank = saves[save][valueKey];
                _saves.Add(save, saveRank);
            }
        }

        private void CollateItemData(List<Dictionary<string, dynamic>> itemData)
        {
            Dictionary<string, dynamic> classData = itemData.First(x => x["type"] == "class");
            _class = classData["name"];
            Dictionary<string, dynamic> ancestryData = itemData.First(x => x["type"] == "ancestry");
            _ancestry = ancestryData["name"];
            Dictionary<string, dynamic> heritageData = itemData.First(x => x["type"] == "heritage");
            _heritage = heritageData["name"];
            Dictionary<string, dynamic> backgroundData = itemData.First(
                x => x["type"] == "background"
            );
            _background = backgroundData["name"];

            foreach (var item in itemData)
            {
                if (item["type"] == "lore")
                {
                    uint loreRank = item["system"]["proficient"]["value"];
                    _lore.Add(item["name"], loreRank);
                }
            }

            foreach (var item in itemData)
            {
                if (item["type"] == "treasure")
                {
                    double value = item["system"]["quantity"];
					switch (item["name"]) {
						case "Platinum Pieces": value *= 10; break;
						case "Silver Pieces": value *= 0.1; break;
						case "Copper Pieces": value *= 0.01; break;
					}
					_coin += value;
                }
            }

            foreach (var item in itemData)
            {
                if (item["type"] == "spell")
                {
                    _spells.Add(item["name"]);
                }
            }

            foreach (var item in itemData)
            {
                if (item["type"] == "feat")
                {
                    _feats.Add(item["name"]);
                }
            }
        }

        private readonly string _name;
        private string _description = "";
        private string _backstory = "";
        private string _birthplace = "";
        private uint _age = 0;
        private string _height = "";
        private string _weight = "";
        private string _gender = "";
        private string _ethnicity = "";
        private string _nationality = "";
        private string _deity = "";
        private string _class = "";
        private string _ancestry = "";
        private string _heritage = "";
        private string _background = "";
		private double _coin = 0;
        private readonly List<string> _languages = new();
        private readonly List<string> _edicts = new();
        private readonly List<string> _anathema = new();
        private readonly Dictionary<string, uint> _skills = new();
        private readonly Dictionary<string, uint> _lore = new();
        private readonly Dictionary<string, uint> _saves = new();
        private readonly List<string> _feats = new();
        private readonly List<string> _spells = new();

        private readonly List<string> TypeFilter =
            new() { "feat", "lore", "class", "ancestry", "heritage", "background", "spell", "treasure" };

        public Dictionary<string, dynamic>? GetCharacterData()
        {
            return null;
        }
    }
}
