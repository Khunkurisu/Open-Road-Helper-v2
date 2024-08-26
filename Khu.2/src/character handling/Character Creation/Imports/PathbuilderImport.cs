namespace OpenRoadHelper.Characters
{
    public class PathbuilderImport : IImportable
    {
        public PathbuilderImport(Dictionary<string, dynamic> jsonData)
        {
            jsonData.Remove("success");
            Dictionary<string, dynamic> buildData = jsonData["build"].ToObject<
                Dictionary<string, dynamic>
            >();

            _name = buildData["name"];
            _class = buildData["class"];
            _ancestry = buildData["ancestry"];
            _heritage = buildData["heritage"];
            _background = buildData["background"];
            _deity = buildData["deity"];
            if (buildData.TryGetValue("height", out dynamic? height))
            {
                _height = height;
            }
            if (buildData.TryGetValue("weight", out dynamic? weight))
            {
                _weight = weight;
            }
            _age =
                buildData["age"] == "not set"
                || buildData["age"] == "Not set"
                || buildData["age"] == ""
                    ? (uint)18
                    : uint.Parse(buildData["age"]);
            _gender = buildData["gender"];
            long level = buildData["level"];
            _level = (uint)level;

            CollateProficiencies(buildData);
            CollateLanguages(buildData);
            CollateSpells(buildData);
            CollateCoinage(buildData);
            CollateAttributeData(buildData);

            UpdateProficiencies();
        }

        public void UpdateProficiencies()
        {
            foreach (string skill in _skills.Keys)
            {
                _skills[skill] = SkillBonus(skill);
            }
        }
        private int SkillBonus(string skillName)
        {
            int skillRank = _skills[skillName] / 2;
            int attributeBonus = GetAttributeBonus(skillName);
            int levelBonus = LevelBonus(skillRank);
            return _skills[skillName] + levelBonus + attributeBonus;
        }

        private int GetAttributeBonus(string skillName)
        {
            if (new List<string> { "athletics" }.Contains(skillName))
            {
                return PF2E.AttributeToModifier(_attributes["str"]);
            }
            else if (new List<string> { "acrobatics", "stealth", "thievery" }.Contains(skillName))
            {
                return PF2E.AttributeToModifier(_attributes["dex"]);
            }
            else if (
                new List<string> { "arcana", "crafting", "occultism", "society" }.Contains(
                    skillName
                )
            )
            {
                return PF2E.AttributeToModifier(_attributes["int"]);
            }
            else if (
                new List<string> { "medicine", "nature", "religion", "survival" }.Contains(
                    skillName
                )
            )
            {
                return PF2E.AttributeToModifier(_attributes["wis"]);
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
                return PF2E.AttributeToModifier(_attributes["cha"]);
            }
            else if (skillName == "fortitude")
            {
                return PF2E.AttributeToModifier(_attributes["con"]);
            }
            else if (skillName == "will")
            {
                return PF2E.AttributeToModifier(_attributes["wis"]);
            }
            else if (skillName == "reflex")
            {
                return PF2E.AttributeToModifier(_attributes["dex"]);
            }
            else if (skillName == "perception")
            {
                return PF2E.AttributeToModifier(_attributes["wis"]);
            }
            else
            {
                return PF2E.AttributeToModifier(_attributes["int"]);
            }
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

        public void CollateCoinage(Dictionary<string, dynamic> jsonData)
        {
            Dictionary<string, double> coins = jsonData["money"].ToObject<
                Dictionary<string, double>
            >();
            foreach (string c in coins.Keys)
            {
                double value = coins[c];
                switch (c)
                {
                    case "cp":
                        value *= 0.01;
                        break;
                    case "sp":
                        value *= 0.1;
                        break;
                    case "pp":
                        value *= 10.0;
                        break;
                }
                _coin += value;
            }
        }

        public void CollateSpells(Dictionary<string, dynamic> jsonData)
        {
            List<Dictionary<string, dynamic>> spellcasting = jsonData["spellCasters"].ToObject<
                List<Dictionary<string, dynamic>>
            >();
            foreach (Dictionary<string, dynamic> spellcaster in spellcasting)
            {
                List<Dictionary<string, dynamic>> spellLists = spellcaster["spells"].ToObject<
                    List<Dictionary<string, dynamic>>
                >();

                foreach (Dictionary<string, dynamic> spellList in spellLists)
                {
                    List<string> spells = spellList["list"].ToObject<List<string>>();
                    foreach (string spell in spells)
                    {
                        _spells.Add(spell);
                    }
                }
            }

            Dictionary<string, dynamic> focusTraditions = jsonData["focus"].ToObject<
                Dictionary<string, dynamic>
            >();
            foreach (string t in focusTraditions.Keys)
            {
                Dictionary<string, dynamic> focusAttributes = focusTraditions[t].ToObject<
                    Dictionary<string, dynamic>
                >();
                foreach (string a in focusAttributes.Keys)
                {
                    Dictionary<string, dynamic> focusAttribute = focusAttributes[a].ToObject<
                        Dictionary<string, dynamic>
                    >();
                    List<string> focusSpells = focusAttribute["focusSpells"].ToObject<
                        List<string>
                    >();
                    foreach (string f in focusSpells)
                    {
                        _spells.Add(f);
                    }
                }
            }
        }

        public void CollateProficiencies(Dictionary<string, dynamic> jsonData)
        {
            Dictionary<string, int> proficiencies = jsonData["proficiencies"].ToObject<
                Dictionary<string, int>
            >();

            _perception = proficiencies["perception"];

            foreach (string k in ProficiencyFilters)
            {
                proficiencies.Remove(k);
            }

            foreach (string p in proficiencies.Keys)
            {
                if (SaveKeys.Contains(p))
                {
                    _saves[p] = proficiencies[p];
                }
                else
                {
                    _skills[p] = proficiencies[p];
                }
            }

            List<List<dynamic>> lores = jsonData["lores"].ToObject<List<List<dynamic>>>();

            foreach (List<dynamic> entry in lores)
            {
                string loreName = entry[0];
                long loreValue = entry[1];
                _lore[loreName] = (int)loreValue;
            }
        }

        public void CollateLanguages(Dictionary<string, dynamic> jsonData)
        {
            List<string> languages = jsonData["languages"].ToObject<List<string>>();

            foreach (string l in languages)
            {
                if (l.StartsWith("Pathbuilder"))
                {
                    continue;
                }
                _languages.Add(l);
            }
        }

        public void CollateAttributeData(Dictionary<string, dynamic> jsonData)
        {
            Dictionary<string, dynamic> abilityData = jsonData["abilities"].ToObject<
                Dictionary<string, dynamic>
            >();

            foreach (string ability in abilityData.Keys)
            {
                if (ability == "breakdown")
                {
                    continue;
                }

                long abilityScore = abilityData[ability];
                _attributes[ability] = (uint)abilityScore;
            }
        }

        public void CollateFeats(Dictionary<string, dynamic> jsonData)
        {
            List<List<dynamic>> feats = jsonData["feats"].ToObject<List<List<dynamic>>>();
            foreach (List<dynamic> feat in feats)
            {
                string name = feat[0];
                if (FeatsWithFollowup.Contains(name))
                {
                    name += " (" + feat[1] + ")";
                }
                _feats.Add(name);
            }
        }

        private readonly string _name;
        private uint _level = 1;
        private uint _age = 0;
        private string _gender = "";
        private string _deity = "";
        private string _class = "";
        private string _ancestry = "";
        private string _heritage = "";
        private float _height = 0;
        private float _weight = 0;
        private string _background = "";
        private double _coin = 0;
        private int _perception = 0;
        private readonly List<string> _languages = new();
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

        private readonly List<string> ProficiencyFilters =
            new()
            {
                "classDC",
                "heavy",
                "medium",
                "light",
                "unarmored",
                "advanced",
                "martial",
                "simple",
                "unarmed",
                "castingArcane",
                "castingDivine",
                "castingOccult",
                "castingPrimal",
                "perception"
            };

        private readonly List<string> FeatsWithFollowup =
            new() { "Canny Acumen", "Assurance", "Additional Lore" };
        private readonly List<string> SaveKeys = new() { "reflex", "will", "fortitude" };

        public Dictionary<string, dynamic>? GetCharacterData()
        {
            return new()
            {
                { "type", ImportType },
                { "name", _name },
                { "level", _level },
                { "age", _age },
                { "gender", _gender },
                { "deity", _deity },
                { "class", _class },
                { "ancestry", _ancestry },
                { "heritage", _heritage },
                { "height", _height },
                { "weight", _weight },
                { "background", _background },
                { "coin", _coin },
                { "perception", _perception },
                { "languages", _languages },
                { "skills", _skills },
                { "lore", _lore },
                { "saves", _saves },
                { "feats", _feats },
                { "spells", _spells },
                { "attributes", _attributes }
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

        public ImportType ImportType { get; } = ImportType.Pathbuilder;
    }
}
