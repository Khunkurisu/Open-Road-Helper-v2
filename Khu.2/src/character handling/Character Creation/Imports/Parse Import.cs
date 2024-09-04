namespace OpenRoadHelper.Characters
{
    public partial class Character
    {
        private void ParseImport(Dictionary<string, dynamic> data)
        {
            _importType = data["type"];
            if (_importType == ImportType.Foundry)
            {
                _height = Generic.GetHeightFromString(data["height"]);
                _weight = Generic.GetWeightFromString(data["weight"]);
            }

            _name = data["name"];
            _desc = data["description"];
            _class = data["class"];
            _ancestry = data["ancestry"];
            _heritage = data["heritage"];
            _background = data["background"];
            _deity = data["deity"];
            _gender = data["gender"];
            _age = data["age"];
            int perception = data["perception"];
            _perception = (uint)perception;
            _currency = data["coin"];
            _languages = data["languages"];
            _skills = data["skills"];
            _lore = data["lore"];
            _saves = data["saves"];
            _feats = data["feats"];
            _spells = data["spells"];
            _attributes = data["attributes"];
            if (data.TryGetValue("foundry image", out dynamic? foundryImage))
            {
                if (_avatars.ContainsKey("FoundryVTT"))
                {
                    _avatars["FoundryVTT"] = foundryImage;
                }
                else
                {
                    _avatars.Add("FoundryVTT", foundryImage);
                }
            }
            if (data.TryGetValue("avatar", out dynamic? initialImage))
            {
                if (_avatars.ContainsKey("Initial"))
                {
                    _avatars["Initial"] = initialImage;
                }
                else
                {
                    _avatars.Add("Initial", initialImage);
                }
                Avatar = GetAvatarIndex("Initial");
                Console.WriteLine(Avatars.ToString());
            }
            if (data.TryGetValue("height", out dynamic? height))
            {
                if (height is not string)
                {
                    _height = height;
                }
            }
            if (data.TryGetValue("weight", out dynamic? weight))
            {
                if (weight is not string)
                {
                    _weight = weight;
                }
            }

            if (data.TryGetValue("edicts", out dynamic? edicts))
            {
                _edicts = edicts;
            }

            if (data.TryGetValue("anathema", out dynamic? anathema))
            {
                _anathema = anathema;
            }
            _ancestry = _ancestry.Replace(" (Tiny)", "");
            _ancestry = _ancestry.Replace(" (Small)", "");
            _ancestry = _ancestry.Replace(" (Medium)", "");
            _ancestry = _ancestry.Replace(" (Large)", "");

            if (Heritage.Contains(_ancestry))
            {
                _heritage = _heritage.Replace(_ancestry, "");
                _heritage = _heritage.Trim();
            }
        }
    }
}
