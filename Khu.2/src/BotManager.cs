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
                await Task.Delay(TimeSpan.FromSeconds(0.5));
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
            await Task.Run(RefreshLoop);
            await Task.CompletedTask;
        }

        private long _lastRefresh = 0;

        private async Task RefreshLoop()
        {
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - _lastRefresh >= 300)
            {
                _lastRefresh = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                foreach (Guild guild in Guilds)
                {
                    await guild.RefreshCharacterPosts();
                    await guild.RefreshQuestPosts();
                }
            }
            await Task.Delay(10);
        }

        private async Task OnButtonExecuted(SocketMessageComponent button)
        {
            if (button.GuildId == null)
            {
                await button.RespondAsync("This must be run in a guild.", ephemeral: true);
                return;
            }
            ulong guildId = (ulong)button.GuildId;
            Guild guild = GetGuild(guildId);

            List<dynamic> formValues = guild.GetFormValues(button.Data.CustomId);

            string actionContext = formValues[0];
            if (actionContext.Contains("createQuest"))
            {
                if (actionContext.Contains("back"))
                {
                    await QuestCreateBack(button);
                }
                else if (actionContext.Contains("cancel"))
                {
                    await QuestCreateCancel(button);
                }
                else if (actionContext.Contains("confirm"))
                {
                    await QuestCreateConfirm(button);
                }
            }
            else if (actionContext.Contains("createCharacter"))
            {
                actionContext = formValues[3];
                if (actionContext.Contains("cancel"))
                {
                    await CharacterCreateCancel(button);
                }
                else if (actionContext.Contains("confirm"))
                {
                    await CharacterCreateConfirm(button);
                }
            }
            else if (actionContext.Contains("replaceCharacter"))
            {
                actionContext = formValues[3];
                if (actionContext.Contains("cancel"))
                {
                    await Character.CharacterReplaceCancel(button);
                }
                else if (actionContext.Contains("confirm"))
                {
                    await Character.CharacterReplaceConfirm(button);
                }
            }
            else if (actionContext.Contains("forceCreateCharacter"))
            {
                actionContext = formValues[3];
                if (actionContext.Contains("cancel"))
                {
                    await CharacterCreateCancel(button);
                }
                else if (actionContext.Contains("confirm"))
                {
                    await CharacterCreateConfirm(button, true);
                }
            }
            else if (actionContext.Contains("editCharacter"))
            {
                actionContext = formValues[3];
                if (actionContext.Contains("cancel"))
                {
                    await CharacterEditCancel(button);
                }
                else if (actionContext.Contains("confirm"))
                {
                    await CharacterEditConfirm(button);
                }
                else
                {
                    await CharacterEditStart(button);
                }
            }
            else if (actionContext.Contains("judgeCharacter"))
            {
                actionContext = formValues[3];
                if (actionContext.Contains("approve"))
                {
                    await ApproveCharacter(button);
                }
                else if (actionContext.Contains("reject"))
                {
                    await RejectCharacter(button);
                }
                else
                {
                    await JudgeCharacter(button);
                }
            }
            else if (actionContext.Contains("refundCharacter"))
            {
                await PendingCharacterRefund(button, actionContext.Contains("FromForced"));
            }
            else if (actionContext.Contains("retireCharacter"))
            {
                actionContext = formValues[3];
                if (actionContext.Contains("cancel"))
                {
                    await RetireCharacterCancel(button);
                }
                else if (actionContext.Contains("confirm"))
                {
                    await RetireCharacterConfirm(button);
                }
                else
                {
                    await RetireCharacter(button);
                }
            }
        }

        private async Task OnSelectMenuExecuted(SocketMessageComponent selectMenu)
        {
            if (selectMenu.GuildId == null)
            {
                await selectMenu.RespondAsync("This must be run in a guild.", ephemeral: true);
                return;
            }
            ulong guildId = (ulong)selectMenu.GuildId;
            Guild guild = GetGuild(guildId);

            List<dynamic> formValues = guild.GetFormValues(selectMenu.Data.CustomId);

            string actionContext = formValues[0];
            if (actionContext.Contains("createQuest"))
            {
                await QuestCreate(selectMenu);
            }
            else if (actionContext.Contains("charDisplay"))
            {
                await DrawCharacterPost(selectMenu, true);
            }
            else if (actionContext.Contains("retirementType"))
            {
                await RetireCharacter(selectMenu, true);
            }
        }

        public static async Task AwakenThread(Character character)
        {
            IThreadChannel? threadChannel = GetThreadChannel(
                character.Guild,
                character.CharacterThread
            );
            if (threadChannel == null)
            {
                return;
            }
            IUser? user = GetGuildUser(character.Guild, character.User);
            if (user == null)
            {
                return;
            }
            await threadChannel.ModifyAsync(x => x.Archived = false);
        }

        public static async Task AwakenThread(Quest quest)
        {
            IThreadChannel? threadChannel = GetThreadChannel(quest.GuildId, quest.ThreadId);
            if (threadChannel == null)
            {
                return;
            }
            IUser? user = GetGuildUser(quest.GuildId, quest.GameMaster);
            if (user == null)
            {
                return;
            }
            await threadChannel.ModifyAsync(x => x.Archived = false);
        }

        private async Task OnModalSubmit(SocketModal modal)
        {
            if (modal.GuildId == null)
            {
                await modal.RespondAsync("This must be run in a guild.", ephemeral: true);
                return;
            }
            ulong guildId = (ulong)modal.GuildId;
            Guild guild = GetGuild(guildId);

            List<dynamic> formValues = guild.GetFormValues(modal.Data.CustomId);

            string actionContext = formValues[0];
            List<SocketMessageComponentData> components = modal.Data.Components.ToList();
            if (actionContext.Contains("createQuest"))
            {
                await QuestCreate(modal, components);
            }
            else if (actionContext.Contains("createCharacter"))
            {
                await CharacterCreate(modal, components);
            }
            else if (actionContext.Contains("replaceCharacter"))
            {
                await Character.ReplaceCharacter(modal, components);
            }
            else if (actionContext.Contains("forceCreateCharacter"))
            {
                await CharacterCreate(modal, components, true);
            }
            else if (actionContext.Contains("rejectCharacter"))
            {
                await RejectCharacter(modal, components);
            }
        }
    }
}
