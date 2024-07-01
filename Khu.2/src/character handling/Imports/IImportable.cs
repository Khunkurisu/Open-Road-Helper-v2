namespace Bot.Characters
{
    public interface IImportable
    {
        public Dictionary<string, dynamic>? GetCharacterData();

        public ImportType ImportType { get; }
    }

    public enum ImportType
    {
        Foundry,
        Pathbuilder
    }
}
