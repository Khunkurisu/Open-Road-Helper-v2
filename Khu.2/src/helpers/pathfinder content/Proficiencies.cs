namespace OpenRoadHelper
{
    public static partial class PF2E
    {
        public static int SkillBonus(
            string skillName,
            int level,
            List<string> feats,
            Dictionary<string, uint> attributes,
            Dictionary<string, int> skills
        )
        {
            int skillRank = skills[skillName] / 2;
            int attributeBonus = GetAttributeBonus(skillName, attributes);
            int levelBonus = LevelBonus(skillRank, level, feats);
            return skills[skillName] + levelBonus + attributeBonus;
        }

        public static int SaveBonus(
            string saveName,
            int level,
            List<string> feats,
            Dictionary<string, uint> attributes,
            Dictionary<string, int> saves
        )
        {
            int saveRank = saves[saveName] / 2;
            int attributeBonus = GetAttributeBonus(saveName, attributes);
            int levelBonus = LevelBonus(saveRank, level, feats);
            return saves[saveName] + levelBonus + attributeBonus;
        }

        public static int PerceptionBonus(
            int perception,
            int level,
            List<string> feats,
            Dictionary<string, uint> attributes
        )
        {
            int perceptionRank = perception / 2;
            int attributeBonus = GetAttributeBonus("perception", attributes);
            int levelBonus = LevelBonus(perceptionRank, level, feats);
            return perception + levelBonus + attributeBonus;
        }

        public static int LevelBonus(int rank, int level, List<string> feats)
        {
            if (rank > 0)
            {
                return level;
            }
            if (feats.Contains("Untrained Improvisation"))
            {
                return Math.Max((level < 7) ? (level < 5 ? level - 2 : level - 1) : level, 0);
            }
            return 0;
        }

        public static int GetAttributeBonus(string profName, Dictionary<string, uint> attributes)
        {
            if (new List<string> { "athletics" }.Contains(profName))
            {
                return AttributeToModifier(attributes["str"]);
            }
            else if (new List<string> { "acrobatics", "stealth", "thievery" }.Contains(profName))
            {
                return AttributeToModifier(attributes["dex"]);
            }
            else if (
                new List<string> { "arcana", "crafting", "occultism", "society" }.Contains(profName)
            )
            {
                return AttributeToModifier(attributes["int"]);
            }
            else if (
                new List<string> { "medicine", "nature", "religion", "survival" }.Contains(profName)
            )
            {
                return AttributeToModifier(attributes["wis"]);
            }
            else if (
                new List<string>
                {
                    "deception",
                    "diplomacy",
                    "intimidation",
                    "performance"
                }.Contains(profName)
            )
            {
                return AttributeToModifier(attributes["cha"]);
            }
            else if (profName == "fortitude")
            {
                return AttributeToModifier(attributes["con"]);
            }
            else if (profName == "will")
            {
                return AttributeToModifier(attributes["wis"]);
            }
            else if (profName == "reflex")
            {
                return AttributeToModifier(attributes["dex"]);
            }
            else if (profName == "perception")
            {
                return AttributeToModifier(attributes["wis"]);
            }
            else
            {
                return AttributeToModifier(attributes["int"]);
            }
        }
    }
}
