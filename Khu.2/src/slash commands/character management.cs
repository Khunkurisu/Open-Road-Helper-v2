using Bot.Guilds;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Bot.Characters
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
            if (autocompleteInteraction.Data.CommandName.Contains("update"))
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
            float height,
            float weight,
            int pathbuilderId = -1
        )
        {
            ulong guildId = Context.Guild.Id;
            IUser player = Context.User;
            string json = "";
            int sheetType = 0;
            HttpClient client = new() { Timeout = TimeSpan.FromSeconds(2) };

            if (pathbuilderId > 0)
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
                        Manager.StoreTempCharacter(characterImport, guildId, player);
                    }
                    else if (sheetType == 2)
                    {
                        PathbuilderImport characterImport = new(importData);
                        Manager.StoreTempCharacter(
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
                        return;
                    }
                    Guild guild = Manager.GetGuild(guildId);
                    Dictionary<string, dynamic>? charData = guild
                        .GetCharacterJson(player.Id)
                        ?.GetCharacterData();
                    string charName = string.Empty;
                    if (charData != null && charData.ContainsKey("name"))
                    {
                        charName = charData["name"];
                    }
                    string charDesc = string.Empty;
                    if (charData != null && charData.ContainsKey("description"))
                    {
                        charDesc = charData["description"];
                    }

                    var textBox = new ModalBuilder()
                        .WithTitle("Create Character")
                        .WithCustomId("createCharacter+" + guildId + "+" + player.Id)
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
