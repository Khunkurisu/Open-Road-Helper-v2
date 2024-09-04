using OpenRoadHelper.Guilds;
using Discord;
using Discord.Interactions;
using Newtonsoft.Json;

namespace OpenRoadHelper.Characters
{
    public class CharacterNameAutocompleteHandler : AutocompleteHandler
    {
        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            Guild guild = Manager.GetGuild(context.Guild.Id);
            IUser? user;
            if (autocompleteInteraction.Data.CommandName.Contains("character"))
            {
                user = context.User;
            }
            else
            {
                user = Manager.GetGuildUser(
                    guild.Id,
                    ulong.Parse(
                        autocompleteInteraction.Data.Options
                            .First(x => x.Name == "user")
                            .Value.ToString() ?? "0"
                    )
                );
            }
            if (user == null)
            {
                return AutocompletionResult.FromError(
                    InteractionCommandError.BadArgs,
                    "could not find user"
                );
            }

            List<AutocompleteResult> results = new();
            foreach (Character character in guild.GetCharacters(user))
            {
                results.Add(new(character.Name, character.Name));
                await Task.Yield();
            }
            return AutocompletionResult.FromSuccess(results.Take(25));
        }
    }

    [Group("character", "Manage your characters with these commands.")]
    public class CharacterManagement : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("create", "Create a character.")]
        public async Task CreateCharacter(
            [Summary("Height", "Character's height in centimeters.")] float height,
            [Summary("Weight", "Character's weight in kilograms.")] float weight,
            [Summary("PathbuilderID", "Export ID for Pathbuilder export.")] uint pathbuilderId,
            [Summary("Avatar", "Image to be associated with the character.")] IAttachment image
        )
        {
            ulong guildId = Context.Guild.Id;
            IUser player = Context.User;

            HttpClient httpClient = new() { Timeout = TimeSpan.FromSeconds(3) };

            if (pathbuilderId <= 0)
            {
                await RespondAsync("Please provide a valid import code.", ephemeral: true);
                return;
            }

            string json = await httpClient.GetStringAsync(
                "https://pathbuilder2e.com/json.php?id=" + pathbuilderId
            );
            if (json == null || json == string.Empty)
            {
                await RespondAsync("Please provide a valid import code.", ephemeral: true);
                return;
            }
            Dictionary<string, dynamic>? importData = JsonConvert.DeserializeObject<
                Dictionary<string, dynamic>
            >(json);
            if (importData == null)
            {
                await RespondAsync("Please provide a valid import code.", ephemeral: true);
                return;
            }

            PathbuilderImport characterImport = new(importData);
            characterImport.AddValue("height", height);
            characterImport.AddValue("weight", weight);
            Manager.StoreTempCharacter(characterImport, guildId, player);

            Guild guild = Manager.GetGuild(guildId);
            Dictionary<string, dynamic>? charData = guild
                .GetCharacterJson(player.Id)
                ?.GetCharacterData();
            if (charData == null)
            {
                await RespondAsync("Failed to process pathbuilder data.", ephemeral: true);
                return;
            }

            string charName = string.Empty;
            string charDesc = string.Empty;
            if (charData.ContainsKey("name"))
            {
                charName = charData["name"];
            }
            if (charData.ContainsKey("description"))
            {
                charDesc = charData["description"];
            }

            string filepath = @".\data\images\characters\";
            Directory.CreateDirectory(filepath);
            string url = image.Url;
            string filename = $"{charName}--initial.webp";
            if (!await Generic.DownloadImage(url, filepath + filename))
            {
                await RespondAsync("Failed to process image.", ephemeral: true);
                return;
            }
            characterImport.AddValue("avatar", filename);

            var textBox = new ModalBuilder()
                .WithTitle("Create Character")
                .WithCustomId(
                    guild.GenerateFormValues(
                        new($"createCharacter", player.Id, string.Empty, string.Empty, new())
                    )
                )
                .AddTextInput(
                    "Name",
                    "character_name",
                    placeholder: "Character Name",
                    value: charName == string.Empty ? null : charName
                )
                .AddTextInput(
                    "Description",
                    "character_description",
                    TextInputStyle.Paragraph,
                    "The character's outward appearance and general description.",
                    value: charDesc == string.Empty ? null : charDesc
                )
                .AddTextInput(
                    "Reputation",
                    "character_reputation",
                    TextInputStyle.Paragraph,
                    "The character's public reputation and info."
                );

            await RespondWithModalAsync(textBox.Build());
        }

        [SlashCommand("update", "Update a character.")]
        public async Task UpdateCharacter(
            [Summary("name", "Character's name.")]
            [Autocomplete(typeof(CharacterNameAutocompleteHandler))]
                string charName,
            [Summary("sheet", "Foundry sheet exported from Foundry.")] IAttachment sheet
        )
        {
            ulong guildId = Context.Guild.Id;
            Guild guild = Manager.GetGuild(guildId);
            IUser user = Context.User;

            Character? character = guild.GetCharacter(user.Id, charName);
            if (character == null)
            {
                await RespondAsync(
                    $"Unable to locate {charName} for user <@{user.Id}>.",
                    ephemeral: true
                );
                return;
            }
            if (sheet != null)
            {
                await Character.ReplaceAsync(Context, sheet, character);
            }
        }

        [SlashCommand("roleplay", "Roleplay as a character.")]
        public async Task Roleplay(
            [Summary("name", "Character's name.")]
            [Autocomplete(typeof(CharacterNameAutocompleteHandler))]
                string charName
        )
        {
            ulong guildId = Context.Guild.Id;
            Guild guild = Manager.GetGuild(guildId);
            IUser player = Context.User;

            var textBox = new ModalBuilder()
                .WithTitle("Roleplay Post")
                .WithCustomId(
                    guild.GenerateFormValues(
                        new($"createRP", player.Id, charName, string.Empty, new())
                    )
                )
                .AddTextInput(
                    "Display Name",
                    "display_name",
                    placeholder: charName,
                    required: false
                )
                .AddTextInput(
                    "Roleplay",
                    "roleplay",
                    TextInputStyle.Paragraph,
                    "Roleplay text content.",
                    required: true
                );

            await Context.Interaction.RespondWithModalAsync(textBox.Build());
        }
    }
}
