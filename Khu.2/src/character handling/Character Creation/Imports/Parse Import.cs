using Bot.Helpers;

namespace Bot.Characters
{
    public partial class Character
    {
        private void ParseImport(Dictionary<string, dynamic> data)
        {
            ImportType importType = data["type"];
            if (importType == ImportType.Foundry)
            {
                _height = GenericHelpers.GetHeightFromString(data["height"]);
                _weight = GenericHelpers.GetWeightFromString(data["weight"]);
            }

            _name = data["name"];
            _desc = data["description"];
            _rep = data["reputation"];
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
            if (data.ContainsKey("foundry image"))
            {
                if (_avatars.ContainsKey("FoundryVTT"))
                {
                    _avatars["FoundryVTT"] = data["foundry image"];
                }
                else
                {
                    _avatars.Add("FoundryVTT", data["foundry image"]);
                }
            }

            if (data.ContainsKey("edicts"))
            {
                _edicts = data["edicts"];
            }

            if (data.ContainsKey("anathema"))
            {
                _anathema = data["anathema"];
            }

            if (Heritage.Contains(_ancestry))
            {
                _heritage = _heritage.Replace(_ancestry, "");
                _heritage = _heritage.TrimEnd();
            }
        }
    }
}
