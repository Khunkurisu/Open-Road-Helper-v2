namespace OpenRoadHelper.Characters
{
    public interface IImportable
    {
        public Dictionary<string, dynamic>? GetCharacterData();

        public void AddValue(string key, dynamic value);

        public ImportType ImportType { get; }
    }

    public enum ImportType
    {
        Foundry,
        Pathbuilder
    }
}
