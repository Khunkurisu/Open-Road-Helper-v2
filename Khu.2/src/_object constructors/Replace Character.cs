using OpenRoadHelper.Guilds;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace OpenRoadHelper.Characters
{
    public partial class Character
    {
        public Character(Character template, Dictionary<string, dynamic> data)
        {
            _id = new(Convert.FromBase64String(template.Id + "=="));
            _user = template._user;
            _guild = template._guild;
            _characterThread = template._characterThread;
            _transactionThread = template._transactionThread;
            _display = template._display;
            _currentAvatar = template._currentAvatar;

            ParseImport(data);

            _name = template._name;
            _updated = template._updated;
            _status = Status.Temp;
            _avatars = template._avatars;
            _colorPref = template._colorPref;
            _notes = template._notes;
            _lastTokenTrade = template._lastTokenTrade;
            _downtime = template._downtime;
            _birthDay = template._birthDay;
            _birthMonth = template._birthMonth;
            _birthYear = template._birthYear;

            _updated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public static async Task ReplaceAsync(
            SocketInteractionContext context,
            IAttachment sheet,
            Character character
        )
        {
            await character.ReplaceAsync(context, sheet);
        }

        public async Task ReplaceAsync(SocketInteractionContext context, IAttachment sheet)
        {
            Guild guild = Manager.GetGuild(Guild);
            IUser? player = Manager.GetGuildUser(Guild, User);
            IUser user = context.User;
            if (player == null)
            {
                await context.Interaction.RespondAsync(
                    "User <@{User}> could not be found in the database.",
                    ephemeral: true
                );
                return;
            }
            if (user.Id != User && !guild.IsGamemaster(user))
            {
                await context.Interaction.RespondAsync(
                    "You lack permission to do that.",
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
                await context.Interaction.RespondAsync(
                    "Please upload a valid json file.",
                    ephemeral: true
                );
                return;
            }

            Dictionary<string, dynamic>? importData = JsonConvert.DeserializeObject<
                Dictionary<string, dynamic>
            >(json);
            if (importData == null)
            {
                await context.Interaction.RespondAsync(
                    "Please upload a valid json file.",
                    ephemeral: true
                );
                return;
            }

            Manager.StoreTempCharacter(new FoundryImport(importData), Guild, player);

            var textBox = new ModalBuilder()
                .WithTitle($"Update {Name}")
                .WithCustomId(
                    guild.GenerateFormValues(
                        new($"replaceCharacter", User, Name, string.Empty, new())
                    )
                )
                .AddTextInput(
                    "Reputation",
                    "character_reputation",
                    TextInputStyle.Paragraph,
                    "The character's public reputation and info.",
                    value: Reputation
                );

            await context.Interaction.RespondWithModalAsync(textBox.Build());
        }

        public static async Task ReplaceCharacter(
            SocketModal modal,
            List<SocketMessageComponentData> components
        )
        {
            string[] modalData = modal.Data.CustomId.Split("+");
            string guildId = modalData[1];
            Guild guild = Manager.GetGuild(ulong.Parse(guildId));
            string playerIdString = modalData[2];
            string charName = modalData[3];
            ulong playerId = ulong.Parse(playerIdString);
            IUser user = modal.User;
            IUser? player = Manager.GetGuildUser(guild.Id, playerId);

            if (player == null)
            {
                await modal.RespondAsync($"Unable to find user.", ephemeral: true);
                return;
            }

            if (player != user && !guild.IsGamemaster(user))
            {
                await modal.RespondAsync(
                    $"You lack permission to perform that action.",
                    ephemeral: true
                );
                return;
            }

            Character? character = guild.GetCharacter(player.Id, charName);
            if (character == null)
            {
                await modal.RespondAsync(
                    $"Unable to locate {charName} for user <@{player.Id}>.",
                    ephemeral: true
                );
                return;
            }

            Dictionary<string, dynamic>? charData = guild
                .GetCharacterJson(player.Id)
                ?.GetCharacterData();
            if (charData == null)
            {
                await modal.RespondAsync($"Unable to load submitted data.", ephemeral: true);
                return;
            }

            string charReputation = components
                .First(x => x.CustomId == "character_reputation")
                .Value;
            Character newCharacter =
                new(character, charData)
                {
                    Reputation = charReputation,
                    Name = $"{charData["name"]}-NEW"
                };
            guild.AddCharacter(playerId, newCharacter);

            await modal.RespondAsync(
                $"Make sure everything looks correct before continuing.",
                embed: newCharacter.GenerateEmbed().Build(),
                components: ConfirmationButtons(
                        guildId,
                        playerId,
                        $"{newCharacter.Name}+{character.Name}",
                        "replaceCharacter"
                    )
                    .Build()
            );
        }

        public static async Task CharacterReplaceConfirm(SocketMessageComponent button)
        {
            if (button.GuildId == null)
            {
                await button.RespondAsync("This must be run in a guild.", ephemeral: true);
                return;
            }
            ulong guildId = (ulong)button.GuildId;
            Guild guild = Manager.GetGuild(guildId);

            FormValue formValues = guild.GetFormValues(button.Data.CustomId);

            ulong playerId = formValues.User;
            IUser user = button.User;
            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await button.RespondAsync("You lack permission to do that!", ephemeral: true);
                return;
            }

            IUser? player = Manager.GetGuildUser(guildId, playerId);
            if (player == null)
            {
                await button.RespondAsync(
                    $"Unable to find user <@{playerId}> in database.",
                    ephemeral: true
                );
                return;
            }

            string[] charNames = formValues.Target.Split("+");
            string charName = charNames[0];
            string oldCharName = charNames[1];

            Character? character = guild.GetCharacter(player.Id, oldCharName);
            if (character == null)
            {
                await button.UpdateAsync(x =>
                {
                    x.Content = $"Unable to locate {oldCharName} for user <@{playerId}>.";
                    x.Components = null;
                    x.Embed = null;
                });
                return;
            }

            IThreadChannel? transThread = Manager.GetThreadChannel(
                guild.Id,
                character.TransactionThread
            );
            if (transThread == null)
            {
                await button.UpdateAsync(x =>
                {
                    x.Content = $"Transaction thread for {character.Name} could not be found.";
                    x.Components = null;
                    x.Embed = null;
                });
                return;
            }

            IThreadChannel? charThread = Manager.GetThreadChannel(
                guild.Id,
                character.CharacterThread
            );
            if (charThread == null)
            {
                await button.UpdateAsync(x =>
                {
                    x.Content = $"Character thread for {character.Name} could not be found.";
                    x.Components = null;
                    x.Embed = null;
                });
                return;
            }

            Character? newCharacter = guild.GetCharacter(playerId, $"{character.Name}-New");
            if (newCharacter == null)
            {
                await button.UpdateAsync(x =>
                {
                    x.Content = $"Unable to load new character data.";
                    x.Components = null;
                    x.Embed = null;
                });
                return;
            }

            newCharacter.Name = newCharacter.Name.Replace("-New", string.Empty);
            newCharacter.Status = character.Status;
            guild.RemoveCharacter(character);

            var msg = await transThread.SendMessageAsync($"{newCharacter.Name} has been updated.");
            await transThread.ModifyAsync(x => x.Name = newCharacter.Name);
            await charThread.ModifyAsync(x => x.Name = newCharacter.Name);
            string msgLink = $"https://discord.com/channels/{guildId}/{msg.Channel.Id}/{msg.Id}";
            await button.UpdateAsync(x =>
            {
                x.Content = $"{newCharacter.Name} has been updated. ({msgLink})";
                x.Components = null;
                x.Embed = null;
            });
        }

        public static async Task CharacterReplaceCancel(SocketMessageComponent button)
        {
            if (button.GuildId == null)
            {
                await button.RespondAsync("This must be run in a guild.", ephemeral: true);
                return;
            }
            ulong guildId = (ulong)button.GuildId;
            Guild guild = Manager.GetGuild(guildId);

            FormValue formValues = guild.GetFormValues(button.Data.CustomId);

            ulong playerId = formValues.User;
            IUser user = button.User;
            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await button.RespondAsync("You lack permission to do that!", ephemeral: true);
                return;
            }

            string charName = formValues.Target;
            guild.ClearTempCharacter(user.Id);

            await button.UpdateAsync(x =>
            {
                x.Content = $"Update for {charName.Replace("-New", string.Empty)} canceled.";
                x.Components = null;
                x.Embed = null;
            });
        }
    }
}
