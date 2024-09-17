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
            _level = template._level;
            _updated = template._updated;
            _status = Status.Replacement;
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

        public async Task ReplaceAsync(SocketInteractionContext context, IAttachment sheet)
        {
            Guild guild = Manager.GetGuild(Guild);
            IUser? player = Manager.GetGuildUser(Guild, User);
            IUser user = context.User;
            if (player == null)
            {
                await InteractionErrors.UserNotFound(context.Interaction, user.Id);
                return;
            }
            if (user.Id != User && !guild.IsGamemaster(user))
            {
                await InteractionErrors.MissingPermissions(context.Interaction, user);
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
                Logger.Warn("User uploaded invalid json file.");
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
                Logger.Warn("User uploaded invalid json file.");
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
            if (modal.GuildId == null)
            {
                await InteractionErrors.MustRunInGuild(modal, modal.User);
                return;
            }
            ulong guildId = (ulong)modal.GuildId;
            Guild guild = Manager.GetGuild(guildId);

            FormValue formValues = guild.GetFormValues(modal.Data.CustomId);

            ulong playerId = formValues.User;
            IUser user = modal.User;
            if (user.Id != playerId)
            {
                await InteractionErrors.MissingPermissions(modal, user);
                return;
            }

            string charName = formValues.Target;
            Character? character = Manager.GetCharacter(guildId, playerId, charName);
            if (character == null)
            {
                await InteractionErrors.CharacterNotFound(modal, charName);
                return;
            }

            Dictionary<string, dynamic>? charData = guild
                .GetCharacterJson(playerId)
                ?.GetCharacterData();
            if (charData == null)
            {
                await modal.RespondAsync($"Unable to load submitted data.", ephemeral: true);
                Logger.Warn("Unable to load user-submitted data.");
                return;
            }

            string charReputation = components
                .First(x => x.CustomId == "character_reputation")
                .Value;
            Character newCharacter =
                new(character, charData)
                {
                    Status = Status.Replacement,
                    Reputation = charReputation,
                    Name = $"{charData["name"]}-New"
                };
            guild.AddCharacter(playerId, newCharacter, false);

            await modal.RespondAsync(
                $"Make sure everything looks correct before continuing.",
                embed: newCharacter.GenerateEmbed().Build(),
                components: newCharacter.GenerateComponents(new() { character.Name }).Build(),
                ephemeral: true
            );
        }

        public static async Task CharacterReplaceConfirm(SocketMessageComponent button)
        {
            if (button.GuildId == null)
            {
                await InteractionErrors.MustRunInGuild(button, button.User);
                return;
            }
            ulong guildId = (ulong)button.GuildId;
            Guild guild = Manager.GetGuild(guildId);

            FormValue formValues = guild.GetFormValues(button.Data.CustomId);

            ulong playerId = formValues.User;
            IUser user = button.User;
            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await InteractionErrors.MissingPermissions(button, user);
                return;
            }

            IUser? player = Manager.GetGuildUser(guildId, playerId);
            if (player == null)
            {
                await button.RespondAsync(
                    $"Unable to find user <@{playerId}> in database.",
                    ephemeral: true
                );
                Logger.Warn("Unable to locate relevant user.");
                return;
            }

            string charName = formValues.Target;
            string oldCharName = formValues.Metadata[0];

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

            IThreadChannel? transThread = (IThreadChannel?)
                Manager.GetTextChannel(character.Guild, character.TransactionThread);
            if (transThread == null)
            {
                await InteractionErrors.CharacterThreadNotFound(button, charName, true);
                return;
            }

            IThreadChannel? charThread = (IThreadChannel?)
                Manager.GetTextChannel(character.Guild, character.CharacterThread);
            if (charThread == null)
            {
                await InteractionErrors.CharacterThreadNotFound(button, charName, true);
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
                await InteractionErrors.MustRunInGuild(button, button.User);
                return;
            }
            ulong guildId = (ulong)button.GuildId;
            Guild guild = Manager.GetGuild(guildId);

            FormValue formValues = guild.GetFormValues(button.Data.CustomId);

            ulong playerId = formValues.User;
            IUser user = button.User;
            if (user.Id != playerId && !guild.IsGamemaster(user))
            {
                await InteractionErrors.MissingPermissions(button, user);
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
