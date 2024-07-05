using Bot.Helpers;
using Bot.PF2;

namespace Bot.Characters
{
    public class FoundryImport : IImportable
    {
        public FoundryImport(Dictionary<string, dynamic> jsonData)
        {
            _name = jsonData["name"];

            _description = jsonData["system"]["details"]["biography"]["appearance"];
            _description = _description.Replace("<p>", "");
            _description = _description.Replace("</p>", "\n");

            _image = jsonData["img"];
            _image = _image.Replace("_Character%20Images/_Players/the%20open%20road/", "");

            Task.Run(() => CollateData(jsonData));
        }

        private async Task CollateData(Dictionary<string, dynamic> jsonData)
        {
            jsonData.Remove("prototypeToken");
            jsonData.Remove("type");
            jsonData.Remove("effects");
            jsonData.Remove("img");
            jsonData.Remove("folder");
            jsonData.Remove("flags");
            jsonData.Remove("_stats");
            await Task.Yield();

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
            await Task.Yield();

            _buildData = systemData["build"].ToObject<Dictionary<string, dynamic>>();
            _itemData = jsonData["items"].ToObject<List<Dictionary<string, dynamic>>>();

            List<Dictionary<string, dynamic>> toRemove = new();
            foreach (Dictionary<string, dynamic> dict in _itemData)
            {
                if (dict.ContainsKey("type"))
                {
                    string type = dict["type"];
                    if (!TypeFilter.Contains(type))
                    {
                        toRemove.Add(dict);
                    }
                }
                await Task.Yield();
            }
            foreach (Dictionary<string, dynamic> dict in toRemove)
            {
                _itemData.Remove(dict);
                await Task.Yield();
            }

            CollateAttributeData();
            await Task.Yield();
            CollateItemData();
            await Task.Yield();
            CollateSystemData(systemData);
            await Task.Yield();
            foreach (string feat in _feats)
            {
                CheckFeatEffects(feat);
            }
            foreach (string k in _skills.Keys)
            {
                _skills[k] = SkillBonus(k);
                await Task.Yield();
            }
            foreach (string k in _lore.Keys)
            {
                _lore[k] = LoreBonus(k);
                await Task.Yield();
            }
            foreach (string k in _saves.Keys)
            {
                _saves[k] = SaveBonus(k);
                await Task.Yield();
            }

            int perceptionModifier = (int)
                Enum.Parse<Helper.ProficiencyBonus>(((Helper.Proficiency)_perception).ToString());
            _perception = perceptionModifier + (int)_level + GetAttributeBonus("perception");
        }

        private void CollateAttributeData()
        {
            Dictionary<string, dynamic> attributeData = _buildData["attributes"].ToObject<
                Dictionary<string, dynamic>
            >();
            Dictionary<string, List<string>> boostData = attributeData["boosts"].ToObject<
                Dictionary<string, List<string>>
            >();

            foreach (string level in boostData.Keys)
            {
                foreach (string attribute in boostData[level])
                {
                    uint mod = 2;
                    if (_attributes[attribute] >= 18)
                    {
                        mod = 1;
                    }
                    _attributes[attribute] += mod;
                }
            }

            _ancestryData = _itemData.First(x => x["type"] == "ancestry");
            Dictionary<string, dynamic> systemData = _ancestryData["system"].ToObject<
                Dictionary<string, dynamic>
            >();
            Dictionary<string, dynamic> boostsData = systemData["boosts"].ToObject<
                Dictionary<string, dynamic>
            >();
            foreach (string level in boostsData.Keys)
            {
                Dictionary<string, dynamic> levelData = boostsData[level].ToObject<
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

            Dictionary<string, dynamic> flawsData = systemData["flaws"].ToObject<
                Dictionary<string, dynamic>
            >();
            foreach (string level in flawsData.Keys)
            {
                Dictionary<string, dynamic> levelData = flawsData[level].ToObject<
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
            Dictionary<string, dynamic> details = systemData["details"].ToObject<
                Dictionary<string, dynamic>
            >();

            Dictionary<string, uint> level = details["level"].ToObject<Dictionary<string, uint>>();
            _level = level["value"];

            Dictionary<string, dynamic> biography = details["biography"].ToObject<
                Dictionary<string, dynamic>
            >();

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

            if (details.ContainsKey("age"))
            {
                _age = details["age"]["value"];
            }
            if (details.ContainsKey("height"))
            {
                _height = details["height"]["value"];
            }
            if (details.ContainsKey("weight"))
            {
                _weight = details["weight"]["value"];
            }
            if (details.ContainsKey("gender"))
            {
                _gender = details["gender"]["value"];
            }
            if (details.ContainsKey("ethnicity"))
            {
                _ethnicity = details["ethnicity"]["value"];
            }
            if (details.ContainsKey("nationality"))
            {
                _nationality = details["nationality"]["value"];
            }
            if (details.ContainsKey("deity"))
            {
                _deity = details["deity"]["value"];
            }

            Dictionary<string, Dictionary<string, uint>> skills = systemData["skills"].ToObject<
                Dictionary<string, Dictionary<string, uint>>
            >();
            foreach (string skill in skills.Keys)
            {
                string skillName = GenericHelpers.ExpandSkillAbbreviation(skill);
                uint skillRank = skills[skill]["rank"];
                _skills.Add(skillName, (int)skillRank);
            }

            Dictionary<string, Dictionary<string, int>> saves = systemData["saves"].ToObject<
                Dictionary<string, Dictionary<string, int>>
            >();
            foreach (string save in saves.Keys)
            {
                Dictionary<string, int> saveHolder = saves[save];
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
                int saveRank = saves[save][valueKey];
                _saves.Add(save, saveRank);
            }
        }

        private void CollateItemData()
        {
            Dictionary<string, dynamic> classData = _itemData.First(x => x["type"] == "class");
            _class = classData["name"];
            Dictionary<string, dynamic> classSystem = classData["system"].ToObject<
                Dictionary<string, dynamic>
            >();
            _perception = (int)classSystem["perception"];

            if (classSystem.ContainsKey("boosts"))
            {
                Dictionary<string, dynamic> boostsData = new();
                if (classSystem.ContainsKey("keyability"))
                {
                    boostsData = classSystem["keyability"].ToObject<Dictionary<string, dynamic>>();
                }
                else if (classSystem.ContainsKey("keyAbility"))
                {
                    boostsData = classSystem["keyAbility"].ToObject<Dictionary<string, dynamic>>();
                }

                if (boostsData.ContainsKey("selected"))
                {
                    string attribute = boostsData["selected"];
                    uint mod = 2;
                    if (_attributes[attribute] >= 18)
                    {
                        mod = 1;
                    }
                    _attributes[attribute] += mod;
                }
                else if (boostsData.ContainsKey("value") && boostsData["value"].Any())
                {
                    string[] attributes = boostsData["value"];
                    string attribute = attributes[0];
                    uint mod = 2;
                    if (_attributes[attribute] >= 18)
                    {
                        mod = 1;
                    }
                    _attributes[attribute] += mod;
                }
            }

            _ancestry = _ancestryData["name"];
            Dictionary<string, dynamic> heritageData = _itemData.First(
                x => x["type"] == "heritage"
            );
            _heritage = heritageData["name"];
            Dictionary<string, dynamic> backgroundData = _itemData.First(
                x => x["type"] == "background"
            );
            Dictionary<string, dynamic> backgroundSystem = backgroundData["system"].ToObject<
                Dictionary<string, dynamic>
            >();
            _background = backgroundData["name"];

            if (backgroundSystem.ContainsKey("boosts"))
            {
                Dictionary<string, dynamic> boostsData = backgroundSystem["boosts"].ToObject<
                    Dictionary<string, dynamic>
                >();
                foreach (string level in boostsData.Keys)
                {
                    Dictionary<string, dynamic> levelData = boostsData[level].ToObject<
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
            }

            foreach (var item in _itemData)
            {
                if (item["type"] == "lore")
                {
                    int loreRank = item["system"]["proficient"]["value"];
                    _lore.Add(item["name"], loreRank);
                }
            }

            foreach (var item in _itemData)
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

            foreach (var item in _itemData)
            {
                if (item["type"] == "spell")
                {
                    _spells.Add(item["name"]);
                }
            }

            foreach (var item in _itemData)
            {
                if (item["type"] == "feat")
                {
                    _feats.Add(item["name"]);
                }
            }
        }

        private void CheckFeatEffects(string featName)
        {
            CheckCannyAcumen(featName);
            CheckTrainingRanks(featName);
        }

        private void CheckWillRanks(string featName)
        {
            List<string> willExpertFeats =
                new() { "Bravery", "Iron Will", "Will Expertise", "Stubborn" };
            List<string> willMasterFeats =
                new()
                {
                    "Path to Perfection (Will)",
                    "Second Path to Perfection (Will)",
                    "Indomitable Will",
                    "Resolve",
                    "Walls of Will",
                    "Will of the Pupil",
                    "Performer's Heart",
                    "Divine Will",
                    "Agile Mind",
                    "Prodigious Will",
                    "Resolute Faith",
                    "Wild Willpower",
                    "Shared Resolve"
                };
            List<string> willLegendaryFeats =
                new()
                {
                    "Third Path to Perfection (Will)",
                    "Greater Resolve",
                    "Fortress of Will",
                    "Greater Performer's Heart"
                };

            int save = _saves["will"];
            if (willExpertFeats.Contains(featName))
            {
                save = Math.Max(save, (int)Helper.Proficiency.Expert);
            }
            else if (willMasterFeats.Contains(featName))
            {
                save = Math.Max(save, (int)Helper.Proficiency.Master);
            }
            else if (willLegendaryFeats.Contains(featName))
            {
                save = Math.Max(save, (int)Helper.Proficiency.Legendary);
            }

            _saves["will"] = save;
        }

        private void CheckFortitudeRanks(string featName)
        {
            List<string> fortitudeExpertFeats =
                new()
                {
                    "Great Fortitude",
                    "Magical Fortitude",
                    "Fortitude Expertise",
                    "Rogue Resilience",
                    "First Doctrine (Warpriest)"
                };
            List<string> fortitudeMasterFeats =
                new()
                {
                    "Path to Perfection (Fortitude)",
                    "Second Path to Perfection (Fortitude)",
                    "Juggernaut",
                    "Battle Hardened",
                    "Warden's Endurance",
                    "Kinetic Durability",
                    "Fifth Doctrine (Warpriest)",
                    "Twin Juggernauts"
                };
            List<string> fortitudeLegendaryFeats =
                new()
                {
                    "Third Path to Perfection (Fortitude)",
                    "Greater Juggernaut",
                    "Greater Kinetic Durability"
                };

            int save = _saves["fortitude"];
            if (fortitudeExpertFeats.Contains(featName))
            {
                save = Math.Max(save, (int)Helper.Proficiency.Expert);
            }
            else if (fortitudeMasterFeats.Contains(featName))
            {
                save = Math.Max(save, (int)Helper.Proficiency.Master);
            }
            else if (fortitudeLegendaryFeats.Contains(featName))
            {
                save = Math.Max(save, (int)Helper.Proficiency.Legendary);
            }

            _saves["fortitude"] = save;
        }

        private void CheckReflexRanks(string featName)
        {
            List<string> reflexExpertFeats =
                new()
                {
                    "Lightning Reflexes",
                    "Precognitive Reflexes",
                    "Reflex Expertise",
                    "Shared Reflexes"
                };
            List<string> reflexMasterFeats =
                new()
                {
                    "Path to Perfection (Reflex)",
                    "Second Path to Perfection (Reflex)",
                    "Evasion",
                    "Tempered Reflexes",
                    "Natural Reflexes",
                    "Kinetic Quickness",
                    "Evasive Reflexes"
                };
            List<string> reflexLegendaryFeats =
                new()
                {
                    "Third Path to Perfection (Reflex)",
                    "Greater Natural Reflexes",
                    "Improved Evasion",
                    "Greater Rogue Reflexes"
                };

            int save = _saves["reflex"];
            if (reflexExpertFeats.Contains(featName))
            {
                save = Math.Max(save, (int)Helper.Proficiency.Expert);
            }
            else if (reflexMasterFeats.Contains(featName))
            {
                save = Math.Max(save, (int)Helper.Proficiency.Master);
            }
            else if (reflexLegendaryFeats.Contains(featName))
            {
                save = Math.Max(save, (int)Helper.Proficiency.Legendary);
            }

            _saves["reflex"] = save;
        }

        private void CheckPerceptionRanks(string featName)
        {
            List<string> perceptionExpertFeats =
                new()
                {
                    "Alertness",
                    "Extrasensory Perception",
                    "Perception Expertise",
                    "Shared Vigilance"
                };
            List<string> perceptionMasterFeats =
                new()
                {
                    "Heightened Senses",
                    "Battlefield Surveyor",
                    "Vigilant Senses",
                    "Perception Mastery"
                };
            List<string> perceptionLegendaryFeats =
                new() { "Incredible Senses", "Perception Legend" };

            int proficiency = _perception;
            if (perceptionExpertFeats.Contains(featName))
            {
                proficiency = Math.Max(proficiency, (int)Helper.Proficiency.Expert);
            }
            else if (perceptionMasterFeats.Contains(featName))
            {
                proficiency = Math.Max(proficiency, (int)Helper.Proficiency.Master);
            }
            else if (perceptionLegendaryFeats.Contains(featName))
            {
                proficiency = Math.Max(proficiency, (int)Helper.Proficiency.Legendary);
            }

            _perception = proficiency;
        }

        private void CheckTrainingRanks(string featName)
        {
            CheckFortitudeRanks(featName);
            CheckWillRanks(featName);
            CheckReflexRanks(featName);
            CheckPerceptionRanks(featName);
        }

        private void CheckCannyAcumen(string featName)
        {
            if (featName.Contains("Canny Acumen"))
            {
                if (featName.Contains("Perception"))
                {
                    _perception = Math.Max(_perception, 2);
                }
                else if (featName.Contains("will"))
                {
                    _saves["will"] = Math.Max(_saves["will"], 2);
                }
                else if (featName.Contains("fortitude"))
                {
                    _saves["fortitude"] = Math.Max(_saves["fortitude"], 2);
                }
                else if (featName.Contains("reflex"))
                {
                    _saves["reflex"] = Math.Max(_saves["reflex"], 2);
                }
            }
        }

        private int GetAttributeBonus(string skillName)
        {
            if (new List<string> { "athletics" }.Contains(skillName))
            {
                return Helper.AttributeToModifier(_attributes["str"]);
            }
            else if (new List<string> { "acrobatics", "stealth", "thievery" }.Contains(skillName))
            {
                return Helper.AttributeToModifier(_attributes["dex"]);
            }
            else if (
                new List<string> { "arcana", "crafting", "occultism", "society" }.Contains(
                    skillName
                )
            )
            {
                return Helper.AttributeToModifier(_attributes["int"]);
            }
            else if (
                new List<string> { "medicine", "nature", "religion", "survival" }.Contains(
                    skillName
                )
            )
            {
                return Helper.AttributeToModifier(_attributes["wis"]);
            }
            else if (
                new List<string>
                {
                    "deception",
                    "diplomacy",
                    "intimidation",
                    "performance"
                }.Contains(skillName)
            )
            {
                return Helper.AttributeToModifier(_attributes["cha"]);
            }
            else if (skillName == "fortitude")
            {
                return Helper.AttributeToModifier(_attributes["con"]);
            }
            else if (skillName == "will")
            {
                return Helper.AttributeToModifier(_attributes["wis"]);
            }
            else if (skillName == "reflex")
            {
                return Helper.AttributeToModifier(_attributes["dex"]);
            }
            else if (skillName == "perception")
            {
                return Helper.AttributeToModifier(_attributes["wis"]);
            }
            else
            {
                return Helper.AttributeToModifier(_attributes["int"]);
            }
        }

        private int SkillBonus(string skillName)
        {
            int skillRank = _skills[skillName];
            int skillModifier = Helper.RankToBonus(skillRank);
            int attributeBonus = GetAttributeBonus(skillName);
            return skillModifier + LevelBonus(skillRank) + attributeBonus;
        }

        private int LoreBonus(string loreName)
        {
            int loreRank = _lore[loreName];
            int loreModifier = Helper.RankToBonus(loreRank);
            int attributeBonus = GetAttributeBonus(loreName);
            return loreModifier + LevelBonus(loreRank) + attributeBonus;
        }

        private int SaveBonus(string saveName)
        {
            int saveRank = _saves[saveName];
            int saveModifier = Helper.RankToBonus(saveRank);
            int attributeBonus = GetAttributeBonus(saveName);
            return saveModifier + (int)_level + attributeBonus;
        }

        private int LevelBonus(int rank)
        {
            if (rank > 0)
            {
                return (int)_level;
            }
            if (_feats.Contains("Untrained Improvisation"))
            {
                return (int)
                    Math.Max((_level < 7) ? (_level < 5 ? _level - 2 : _level - 1) : _level, 0);
            }
            return 0;
        }

        private Dictionary<string, dynamic> _buildData = new();
        private Dictionary<string, dynamic> _ancestryData = new();
        private List<Dictionary<string, dynamic>> _itemData = new();

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
        private readonly string _image = string.Empty;
        private readonly List<string> _languages = new();
        private readonly List<string> _edicts = new();
        private readonly List<string> _anathema = new();
        private readonly Dictionary<string, int> _skills = new();
        private readonly Dictionary<string, int> _lore = new();
        private readonly Dictionary<string, int> _saves = new();
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
            return new()
            {
                { "type", ImportType },
                { "name", _name },
                { "level", _level },
                { "description", _description },
                { "backstory", _backstory },
                { "birthplace", _birthplace },
                { "age", _age },
                { "height", _height },
                { "weight", _weight },
                { "gender", _gender },
                { "ethnicity", _ethnicity },
                { "nationality", _nationality },
                { "deity", _deity },
                { "class", _class },
                { "ancestry", _ancestry },
                { "heritage", _heritage },
                { "background", _background },
                { "coin", _coin },
                { "perception", _perception },
                { "languages", _languages },
                { "edicts", _edicts },
                { "anathema", _anathema },
                { "skills", _skills },
                { "lore", _lore },
                { "saves", _saves },
                { "feats", _feats },
                { "spells", _spells },
                { "attributes", _attributes },
                { "foundry image", _image }
            };
        }

        public void AddValue(string key, dynamic value)
        {
            switch (key)
            {
                case "height":
                {
                    _height = value;
                    break;
                }
                case "weight":
                {
                    _weight = value;
                    break;
                }
            }
        }

        public ImportType ImportType { get; } = ImportType.Foundry;
    }
}
