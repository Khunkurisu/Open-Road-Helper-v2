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
            CollateSystemData(systemData);

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
            CollateItemData(itemData);
            CollateAttributeData();
        }

        private void CollateAttributeData()
        {
            Dictionary<string, dynamic> attributeData = _buildData["attributes"].ToObject<
                Dictionary<string, dynamic>
            >();
            Dictionary<string, List<string>> boostData = attributeData["boosts"].ToObject<
                Dictionary<string, List<string>>
            >();

            foreach (string l in boostData.Keys)
            {
                foreach (string attribute in boostData[l])
                {
                    uint mod = 2;
                    if (_attributes[attribute] >= 18)
                    {
                        mod = 1;
                    }
                    _attributes[attribute] += mod;
                }
            }

            Dictionary<string, dynamic> systemData = _ancestryData["system"].ToObject<
                Dictionary<string, dynamic>
            >();
            Dictionary<string, dynamic> boostsData = systemData["boosts"].ToObject<
                Dictionary<string, dynamic>
            >();
            foreach (string l in boostsData.Keys)
            {
                Dictionary<string, dynamic> levelData = boostsData[l].ToObject<
                    Dictionary<string, dynamic>
                >();

                if (levelData.ContainsKey("selected"))
                {
                    string attribute = levelData["selected"];
                    uint mod = 2;
                    if (_attributes[attribute] >= 18)
                    {
                        mod = 1;
                    }
                    _attributes[attribute] += mod;
                }
            }

            Dictionary<string, dynamic> flawsData = systemData["boosts"].ToObject<
                Dictionary<string, dynamic>
            >();
            foreach (string l in flawsData.Keys)
            {
                Dictionary<string, dynamic> levelData = flawsData[l].ToObject<
                    Dictionary<string, dynamic>
                >();

                if (levelData.ContainsKey("selected"))
                {
                    string attribute = levelData["selected"];
                    uint mod = 2;
                    if (_attributes[attribute] > 18)
                    {
                        mod = 1;
                    }
                    _attributes[attribute] -= mod;
                }
            }
        }

        private void CollateSystemData(Dictionary<string, dynamic> systemData)
        {
            _buildData = systemData["build"].ToObject<Dictionary<string, dynamic>>();

            Dictionary<string, dynamic> details = systemData["details"].ToObject<
                Dictionary<string, dynamic>
            >();

            Dictionary<string, uint> level = details["level"].ToObject<Dictionary<string, uint>>();
            _level = level["value"];

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
            Dictionary<string, dynamic> classSystem = classData["system"].ToObject<
                Dictionary<string, dynamic>
            >();
            long perception = classSystem["perception"];
            _perception = (int)perception;

            _ancestryData = itemData.First(x => x["type"] == "ancestry");
            _ancestry = _ancestryData["name"];
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
                    switch (item["name"])
                    {
                        case "Platinum Pieces":
                            value *= 10;
                            break;
                        case "Silver Pieces":
                            value *= 0.1;
                            break;
                        case "Copper Pieces":
                            value *= 0.01;
                            break;
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
                    if (item["name"] == "Canny Acumen (Perception)")
                    {
                        _perception = 2;
                    }
                }
            }
        }

        private Dictionary<string, dynamic> _buildData = new();
        private Dictionary<string, dynamic> _ancestryData = new();

        private readonly string _name;
        private uint _level = 1;
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
        private int _perception = 0;
        private readonly List<string> _languages = new();
        private readonly List<string> _edicts = new();
        private readonly List<string> _anathema = new();
        private readonly Dictionary<string, uint> _skills = new();
        private readonly Dictionary<string, uint> _lore = new();
        private readonly Dictionary<string, uint> _saves = new();
        private readonly List<string> _feats = new();
        private readonly List<string> _spells = new();
        private readonly Dictionary<string, uint> _attributes =
            new()
            {
                { "str", 10 },
                { "dex", 10 },
                { "con", 10 },
                { "int", 10 },
                { "wis", 10 },
                { "cha", 10 }
            };

        private readonly List<string> TypeFilter =
            new()
            {
                "feat",
                "lore",
                "class",
                "ancestry",
                "heritage",
                "background",
                "spell",
                "treasure"
            };

        public Dictionary<string, dynamic>? GetCharacterData()
        {
            return null;
        }
    }
}
