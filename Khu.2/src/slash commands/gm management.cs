using OpenRoadHelper.Characters;
using OpenRoadHelper.Guilds;
using Discord;
using Discord.Interactions;
using Newtonsoft.Json;

namespace OpenRoadHelper.GameMaster
{
    [RequireGM(Group = "gm")]
    [Group("gm", "Perform gm related tasks with these commands.")]
    public partial class GameMasterManagement : InteractionModuleBase<SocketInteractionContext>
    {
        [Group("characters", "Manage various aspects of characters.")]
        public class ManageCharacters : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("force-create", "Forcibly create a character from a foundry import.")]
            public async Task CreateCharacter(IUser player, IAttachment sheet)
            {
                ulong guildId = Context.Guild.Id;
                Guild guild = Manager.GetGuild(guildId);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync(
                        "Only GMs may run this command!",
                        ephemeral: true
                    );
                    return;
                }

                string json = string.Empty;
                HttpClient httpClient = new() { Timeout = TimeSpan.FromSeconds(2) };

                if (sheet != null && sheet.Filename.Contains(".json"))
                {
                    json = await httpClient.GetStringAsync(sheet.Url);
                }
                if (json == null || json == string.Empty)
                {
                    await RespondAsync("Please upload a valid json file.", ephemeral: true);
                    return;
                }

                Dictionary<string, dynamic>? importData = JsonConvert.DeserializeObject<
                    Dictionary<string, dynamic>
                >(json);
                if (importData == null)
                {
                    await RespondAsync("Please upload a valid json file.", ephemeral: true);
                    return;
                }

                FoundryImport characterImport = new(importData);
                Manager.StoreTempCharacter(characterImport, guildId, player);

                Dictionary<string, dynamic>? charData = guild
                    .GetCharacterJson(player.Id)
                    ?.GetCharacterData();
                string charName = string.Empty;
                string charDesc = string.Empty;
                if (charData != null)
                {
                    if (charData.ContainsKey("name"))
                    {
                        charName = charData["name"];
                    }
                    if (charData.ContainsKey("description"))
                    {
                        charDesc = charData["description"];
                    }
                }

                var textBox = new ModalBuilder()
                    .WithTitle("Create Character")
                    .WithCustomId(
                        guild.GenerateFormValues(new() { $"forceCreateCharacter", player.Id })
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
        }

        [Group("currency", "Manage player currency values")]
        public class CurrencyManagement : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("add-pt", "Increase player PT by an amount.")]
            public async Task AddPT(IUser user, uint amount)
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync(
                        "Only GMs may run this command!",
                        ephemeral: true
                    );
                    return;
                }
                uint oldPT = guild.GetPlayerTokenCount(user.Id);
                guild.IncreasePlayerTokenCount(user.Id, amount);
                await Context.Interaction.RespondAsync(
                    $"PT for {user.Username} updated from {oldPT} PT to {guild.GetPlayerTokenCount(user.Id)} PT."
                );
                await guild.RefreshCharacterPosts(user.Id);
            }

            [SlashCommand("sub-pt", "Decrease player PT by an amount.")]
            public async Task SubtractPT(IUser user, uint amount)
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync(
                        "Only GMs may run this command!",
                        ephemeral: true
                    );
                    return;
                }
                uint oldPT = guild.GetPlayerTokenCount(user.Id);
                guild.DecreasePlayerTokenCount(user.Id, amount);
                await Context.Interaction.RespondAsync(
                    $"PT for {user.Username} updated from {oldPT} PT to {guild.GetPlayerTokenCount(user.Id)} PT."
                );
                await guild.RefreshCharacterPosts(user.Id);
            }

            [SlashCommand("set-pt", "Set player PT to a specific amount.")]
            public async Task SetPT(IUser user, uint amount)
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync(
                        "Only GMs may run this command!",
                        ephemeral: true
                    );
                    return;
                }
                uint oldPT = guild.GetPlayerTokenCount(user.Id);
                guild.SetPlayerTokenCount(user.Id, amount);
                await Context.Interaction.RespondAsync(
                    $"PT for {user.Username} updated from {oldPT} PT to {guild.GetPlayerTokenCount(user.Id)} PT."
                );
                await guild.RefreshCharacterPosts(user.Id);
            }

            [SlashCommand("add-gp", "Increase character GP by an amount.")]
            public async Task AddGP(
                IUser user,
                [Autocomplete(typeof(CharacterNameAutocompleteHandler))] string charName,
                float amount
            )
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync(
                        "Only GMs may run this command!",
                        ephemeral: true
                    );
                    return;
                }
                Character? character = guild.GetCharacter(user.Id, charName);
                if (character == null)
                {
                    await Context.Interaction.RespondAsync(
                        $"Unable to find character '{charName}' for user <@{user.Id}>"
                    );
                    return;
                }
                double oldGP = character.Gold;
                character.Gold += amount;
                await Context.Interaction.RespondAsync(
                    $"Gold for {charName} updated from {oldGP} gp to {character.Gold} gp."
                );
                await Manager.DrawCharacterPost(character);
            }

            [SlashCommand("sub-gp", "Decrease character GP by an amount.")]
            public async Task SubtractGP(
                IUser user,
                [Autocomplete(typeof(CharacterNameAutocompleteHandler))] string charName,
                float amount
            )
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync(
                        "Only GMs may run this command!",
                        ephemeral: true
                    );
                    return;
                }
                Character? character = guild.GetCharacter(user.Id, charName);
                if (character == null)
                {
                    await Context.Interaction.RespondAsync(
                        $"Unable to find character '{charName}' for user <@{user.Id}>"
                    );
                    return;
                }
                double oldGP = character.Gold;
                character.Gold -= amount;
                await Context.Interaction.RespondAsync(
                    $"Gold for {charName} updated from {oldGP} gp to {character.Gold} gp."
                );
                await Manager.DrawCharacterPost(character);
            }

            [SlashCommand("set-gp", "Set character GP to a specific amount.")]
            public async Task SetGP(
                IUser user,
                [Autocomplete(typeof(CharacterNameAutocompleteHandler))] string charName,
                float amount
            )
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync(
                        "Only GMs may run this command!",
                        ephemeral: true
                    );
                    return;
                }
                Character? character = guild.GetCharacter(user.Id, charName);
                if (character == null)
                {
                    await Context.Interaction.RespondAsync(
                        $"Unable to find character '{charName}' for user <@{user.Id}>"
                    );
                    return;
                }
                double oldGP = character.Gold;
                character.Gold = amount;
                await Context.Interaction.RespondAsync(
                    $"Gold for {charName} updated from {oldGP} gp to {character.Gold} gp."
                );
                await Manager.DrawCharacterPost(character);
            }

            [SlashCommand("add-dt", "Increase character DT by an amount.")]
            public async Task AddDT(
                IUser user,
                [Autocomplete(typeof(CharacterNameAutocompleteHandler))] string charName,
                uint amount
            )
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync(
                        "Only GMs may run this command!",
                        ephemeral: true
                    );
                    return;
                }
                Character? character = guild.GetCharacter(user.Id, charName);
                if (character == null)
                {
                    await Context.Interaction.RespondAsync(
                        $"Unable to find character '{charName}' for user <@{user.Id}>"
                    );
                    return;
                }
                uint oldDT = character.Downtime;
                character.Downtime += amount;
                await Context.Interaction.RespondAsync(
                    $"Downtime for {charName} updated from {oldDT} DT to {character.Downtime} DT."
                );
                await Manager.DrawCharacterPost(character);
            }

            [SlashCommand("sub-dt", "Decrease character DT by an amount.")]
            public async Task SubtractDT(
                IUser user,
                [Autocomplete(typeof(CharacterNameAutocompleteHandler))] string charName,
                uint amount
            )
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync(
                        "Only GMs may run this command!",
                        ephemeral: true
                    );
                    return;
                }
                Character? character = guild.GetCharacter(user.Id, charName);
                if (character == null)
                {
                    await Context.Interaction.RespondAsync(
                        $"Unable to find character '{charName}' for user <@{user.Id}>"
                    );
                    return;
                }
                uint oldDT = character.Downtime;
                character.Downtime -= amount;
                await Context.Interaction.RespondAsync(
                    $"Downtime for {charName} updated from {oldDT} DT to {character.Downtime} DT."
                );
                await Manager.DrawCharacterPost(character);
            }

            [SlashCommand("set-dt", "Set character DT to a specific amount.")]
            public async Task SetDT(
                IUser user,
                [Autocomplete(typeof(CharacterNameAutocompleteHandler))] string charName,
                uint amount
            )
            {
                Guild guild = Manager.GetGuild(Context.Guild.Id);
                if (!guild.IsGamemaster(Context.User))
                {
                    await Context.Interaction.RespondAsync(
                        "Only GMs may run this command!",
                        ephemeral: true
                    );
                    return;
                }
                Character? character = guild.GetCharacter(user.Id, charName);
                if (character == null)
                {
                    await Context.Interaction.RespondAsync(
                        $"Unable to find character '{charName}' for user <@{user.Id}>"
                    );
                    return;
                }
                uint oldDT = character.Downtime;
                character.Downtime = amount;
                await Context.Interaction.RespondAsync(
                    $"Downtime for {charName} updated from {oldDT} DT to {character.Downtime} DT."
                );
                await Manager.DrawCharacterPost(character);
            }
        }
    }
}
