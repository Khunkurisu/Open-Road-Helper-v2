using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Bot.Characters
{
    [Group("character", "Manage your characters with these commands.")]
    public class CharacterManagement : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("create", "Create a character.")]
        public async Task CreateCharacter(
            float height,
            float weight,
            IAttachment? sheet = null,
            int pathbuilderId = -1
        )
        {
            ulong guildId = Context.Guild.Id;
            IUser player = Context.User;
            string json = "";
            int sheetType = 0;
            HttpClient client = new() { Timeout = TimeSpan.FromSeconds(2) };

            if (sheet != null && sheet.Filename.Contains(".json"))
            {
                json = await client.GetStringAsync(sheet.Url);
                sheetType = 1;
            }
            else if (pathbuilderId > 0)
            {
                json = await client.GetStringAsync(
                    "https://pathbuilder2e.com/json.php?id=" + pathbuilderId
                );
                sheetType = 2;
            }
            if (json != "")
            {
                Dictionary<string, dynamic>? importData = JsonConvert.DeserializeObject<
                    Dictionary<string, dynamic>
                >(json);

                if (importData != null)
                {
                    if (sheetType == 1)
                    {
                        FoundryImport characterImport = new(importData);
                        BotManager.StoreTempCharacter(characterImport, guildId, player);
                    }
                    else if (sheetType == 2)
                    {
                        PathbuilderImport characterImport = new(importData);
                        BotManager.StoreTempCharacter(
                            characterImport,
                            guildId,
                            player,
                            height,
                            weight
                        );
                    }
                    else
                    {
                        await RespondAsync("Please upload a valid json file or import code.");
                    }

                    var textBox = new ModalBuilder()
                        .WithTitle("Create Character")
                        .WithCustomId("createCharacter+" + guildId + "+" + player.Id)
                        .AddTextInput("Name", "character_name", placeholder: "Character Name")
                        .AddTextInput(
                            "Description",
                            "character_description",
                            TextInputStyle.Paragraph,
                            "The character's outward appearance and general description."
                        )
                        .AddTextInput(
                            "Reputation",
                            "character_reputation",
                            TextInputStyle.Paragraph,
                            "The character's public reputation and info."
                        );

                    await RespondWithModalAsync(textBox.Build());
                }
                else
                {
                    await RespondAsync("Please upload a valid json file or import code.");
                }
            }
            else
            {
                await RespondAsync("Please upload a valid json file or import code.");
            }
        }

        [SlashCommand("update", "Update a character.")]
        public async Task UpdateCharacter(string name, IAttachment sheet)
        {
            await RespondAsync(name);
            //ulong messageId = SetToNewMessageID();
            SocketUser user = Context.Client.CurrentUser;
            await ReplyAsync($"{user.Username}#{user.Discriminator}");
        }

        [SlashCommand("roleplay", "Roleplay as a character.")]
        public async Task Roleplay(string name)
        {
            ulong guildId = Context.Guild.Id;
            IUser player = Context.User;

            var textBox = new ModalBuilder()
                .WithTitle("Roleplay Post")
                .WithCustomId("createRP+" + guildId + "+" + player.Username + "+" + name)
                .AddTextInput("Display Name", "display_name", placeholder: name, required: false)
                .AddTextInput(
                    "Roleplay",
                    "roleplay",
                    TextInputStyle.Paragraph,
                    "Roleplay text content."
                );

            await Context.Interaction.RespondWithModalAsync(textBox.Build());
        }
    }

    public class CharacterNameSuggestions : AutocompleteHandler
    {
        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
            IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter,
            IServiceProvider services
        )
        {
            await Task.Yield();
            IEnumerable<AutocompleteResult> results = new[]
            {
                new AutocompleteResult("name1", "value1"),
                new AutocompleteResult("name2", "value2")
            };
            return AutocompletionResult.FromSuccess(results.Take(25));
        }
    }
}
