using Bot.Characters;
using Bot.Guilds;
using Bot.Quests;
using Discord;
using Discord.WebSocket;

namespace Bot
{
    public partial class Manager
    {
        private static DiscordSocketClient? _client;
        public static readonly List<Guild> Guilds = new();

        public static Quest? GetQuest(ulong guildId, Guid questId)
        {
            return GetGuild(guildId).GetQuest(questId);
        }

        public static Character? GetCharacter(ulong guildId, ulong playerId, string charName)
        {
            return GetGuild(guildId).GetCharacter(playerId, charName);
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

        public static SocketThreadChannel? GetThreadChannel(ulong guildId, ulong channelId)
        {
            if (_client != null)
            {
                return _client.GetGuild(guildId).GetThreadChannel(channelId);
            }
            return null;
        }

        public static SocketTextChannel? GetTextChannel(ulong guildId, ulong channelId)
        {
            if (_client != null)
            {
                return _client.GetGuild(guildId).GetTextChannel(channelId);
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

        public static bool IsGamemaster(IUser gm, ulong guild)
        {
            return GetGuild(guild).IsGamemaster(gm);
        }

        public Manager(DiscordSocketClient client)
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
            while (_client == null)
            {
                await Task.Yield();
            }
            foreach (string guildId in guildIds)
            {
                Guild guild = new(ulong.Parse(guildId.Replace(guildData, "")));
                Guilds.Add(guild);
                SocketGuild socketGuild = _client.GetGuild(guild.Id);
                await socketGuild.DownloadUsersAsync();
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
            else if (customId.Contains("createCharacter"))
            {
                if (customId.Contains("cancel"))
                {
                    await CharacterCreateCancel(button);
                }
                else if (customId.Contains("confirm"))
                {
                    await CharacterCreateConfirm(button);
                }
            }
            else if (customId.Contains("editCharacter"))
            {
                if (customId.Contains("cancel"))
                {
                    await CharacterEditCancel(button);
                }
                else if (customId.Contains("confirm"))
                {
                    await CharacterEditConfirm(button);
                }
                else
                {
                    await CharacterEditStart(button);
                }
            }
            else if (customId.Contains("judgeCharacter"))
            {
                if (customId.Contains("approve"))
                {
                    await ApproveCharacter(button);
                }
                else if (customId.Contains("reject"))
                {
                    await RejectCharacter(button);
                }
                else
                {
                    await JudgeCharacter(button);
                }
            }
            else if (customId.Contains("refundCharacter"))
            {
                await PendingCharacterRefund(button);
            }
        }

        private async Task OnSelectMenuExecuted(SocketMessageComponent selectMenu)
        {
            if (selectMenu.Data.CustomId.Contains("createQuest"))
            {
                await QuestCreate(selectMenu);
            }
            else if (selectMenu.Data.CustomId.Contains("charDisplay"))
            {
                await DrawCharacterPost(selectMenu);
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
            else if (modal.Data.CustomId.Contains("rejectCharacter"))
            {
                await RejectCharacter(modal, components);
            }
        }
    }
}
