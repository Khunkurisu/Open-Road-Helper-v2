using Bot.Guilds;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Bot.Characters
{
    public partial class Character
    {
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
            ulong guildId = context.Guild.Id;
            Guild guild = Manager.GetGuild(guildId);
            IUser player = context.User;
            string json = string.Empty;
            HttpClient client = new() { Timeout = TimeSpan.FromSeconds(2) };

            if (sheet != null && sheet.Filename.Contains(".json"))
            {
                json = await client.GetStringAsync(sheet.Url);
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

            Manager.StoreTempCharacter(new FoundryImport(importData), guildId, player);

            var textBox = new ModalBuilder()
                .WithTitle($"Update {Name}")
                .WithCustomId("replaceCharacter+" + guildId + "+" + player.Id + "+" + Name)
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

            Character? character = guild.GetCharacter(guild.Id, modalData[3]);
            if (character == null)
            {
                await modal.RespondAsync(
                    $"Unable to locate {modalData[3]} for user <@{player.Id}>.",
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

            IThreadChannel? transThread = Manager.GetThreadChannel(
                guild.Id,
                character.TransactionThread
            );
            if (transThread == null)
            {
                await modal.RespondAsync(
                    $"Transaction thread for {character.Name} could not be found.",
                    ephemeral: true
                );
                return;
            }

            Character charHolder = new(character);
            character.ParseImport(charData);
            character.Reputation = components
                .First(x => x.CustomId == "character_reputation")
                .Value;
            ;
            character.Name = charHolder.Name;
            guild.QueueSave("characters");

            var msg = await transThread.SendMessageAsync($"{character.Name} has been updated.");
            string msgLink = $"https://discord.com/channels/{guildId}/{msg.Channel.Id}/{msg.Id}";
            await modal.RespondAsync(
                $"{character.Name} has been updated. ({msgLink})",
                ephemeral: true
            );
        }
    }
}