using Bot.Characters;
using Bot.Guilds;
using Bot.Quests;
using Discord;
using Discord.WebSocket;

namespace Bot
{
    public partial class BotManager
    {
        private static DiscordSocketClient? _client;
        public static readonly List<Guild> Guilds = new();

        public static Quest? GetQuest(ulong guildId, Guid questId)
        {
            return GetGuild(guildId).GetQuest(questId);
        }

        public static Character? GetCharacter(ulong guildId, ulong playerId, Guid characterId)
        {
            return GetGuild(guildId).GetCharacter(playerId, characterId);
        }

        public static Character? GetCharacter(ulong guildId, Guid characterId)
        {
            return GetGuild(guildId).GetCharacter(characterId);
        }

        public static IForumChannel? GetForumChannel(ulong guildId, ulong channelId)
        {
            if (_client != null)
            {
                return _client.GetGuild(guildId).GetForumChannel(channelId);
            }
            return null;
        }

        public static IGuildChannel? GetChannel(ulong guildId, ulong channelId)
        {
            if (_client != null)
            {
                return _client.GetGuild(guildId).GetChannel(channelId);
            }
            return null;
        }

        public static IUser? GetGuildUser(ulong guild, ulong user)
        {
            if (_client != null)
            {
                return _client.GetGuild(guild).GetUser(user);
            }
            return null;
        }

        public static Guild GetGuild(ulong id)
        {
            Guild guild;
            try
            {
                guild = Guilds.First(x => x.Id == id);
            }
            catch
            {
                guild = new(id);
                Guilds.Add(guild);
            }
            return guild;
        }

        public BotManager(DiscordSocketClient client)
        {
            _client = client;
            _client.ModalSubmitted += OnModalSubmit;
            _client.SelectMenuExecuted += OnSelectMenuExecuted;
            _client.ButtonExecuted += OnButtonExecuted;
            _client.Ready += OnClientReady;
        }

        private async Task OnClientReady()
        {
            string guildData = @".\data\guilds\";
            string[] guildIds = Directory.GetDirectories(guildData);
            foreach (string guildId in guildIds)
            {
                Guild guild = new(ulong.Parse(guildId.Replace(guildData, "")));
                Guilds.Add(guild);
                guild.LoadAll();
                await Task.Yield();
            }
            await Task.CompletedTask;
        }

        private async Task OnButtonExecuted(SocketMessageComponent button)
        {
            string customId = button.Data.CustomId;
            if (customId.Contains("createQuest"))
            {
                if (customId.Contains("back"))
                {
                    await QuestCreateBack(button);
                }
                else if (customId.Contains("cancel"))
                {
                    await QuestCreateCancel(button);
                }
                else if (customId.Contains("confirm"))
                {
                    await QuestCreateConfirm(button);
                }
            }
        }

        private async Task OnSelectMenuExecuted(SocketMessageComponent selectMenu)
        {
            if (selectMenu.Data.CustomId.Contains("createQuest"))
            {
                await QuestCreate(selectMenu);
            }
        }

        private async Task OnModalSubmit(SocketModal modal)
        {
            List<SocketMessageComponentData> components = modal.Data.Components.ToList();
            if (modal.Data.CustomId.Contains("createQuest"))
            {
                await QuestCreate(modal, components);
            }
            else if (modal.Data.CustomId.Contains("createCharacter"))
            {
                await CharacterCreate(modal, components);
            }
        }
    }
}
