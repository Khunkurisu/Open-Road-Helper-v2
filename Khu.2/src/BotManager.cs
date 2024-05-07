using Bot.Characters;
using Bot.Guilds;
using Bot.Quests;
using Discord;
using Discord.WebSocket;

namespace Bot
{
    public class BotManager
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

        public static void StoreTempCharacter(
            IImportable characterImport,
            ulong guildId,
            IUser player
        )
        {
            Guild guild = GetGuild(guildId);
            if (guild == null)
            {
                return;
            }
            guild.StoreTempCharacter(player.Id, characterImport);
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

        private static async Task QuestCreateConfirm(SocketMessageComponent button)
        {
            IUser user = button.User;
            string gm = button.Data.CustomId.Split("-")[2];
            if (user.Username == gm)
            {
                string guildId = button.Data.CustomId.Split("-")[1];
                Guild guild = GetGuild(ulong.Parse(guildId));

                string name = button.Data.CustomId.Split("-")[3];

                Quest? quest = guild.GetQuest(name, user);
                if (quest != null)
                {
                    await PostQuest(button, guild, quest, user);
                }
                else
                {
                    await button.UpdateAsync(x =>
                    {
                        x.Content = "Somehow Quest was null.";
                    });
                }
            }
            else
            {
                await button.UpdateAsync(x => { });
            }
        }

        private static async Task QuestCreateBack(SocketMessageComponent button)
        {
            IUser user = button.User;
            string gm = button.Data.CustomId.Split("-")[2];
            if (user.Username == gm)
            {
                string guildId = button.Data.CustomId.Split("-")[1];
                Guild guild = GetGuild(ulong.Parse(guildId));

                string name = button.Data.CustomId.Split("-")[3];

                Quest? quest = guild.GetQuest(name, user);
                if (quest != null)
                {
                    var threatMenu = Quest.ThreatSelector(guildId, gm, name);
                    var messageComponents = new ComponentBuilder()
                        .WithSelectMenu(threatMenu)
                        .WithButton(
                            "Back",
                            "createQuest-" + guildId + "-" + gm + "-" + name + "-back",
                            ButtonStyle.Danger
                        );
                    var questEmbed = quest.GenerateEmbed(user);
                    await button.UpdateAsync(x =>
                    {
                        x.Embed = questEmbed.Build();
                        x.Components = messageComponents.Build();
                    });
                }
                else
                {
                    await button.UpdateAsync(x =>
                    {
                        x.Content = "Somehow Quest was null.";
                    });
                }
            }
            else
            {
                await button.UpdateAsync(x => { });
            }
        }

        private static async Task QuestCreateCancel(SocketMessageComponent button)
        {
            IUser user = button.User;
            string gm = button.Data.CustomId.Split("-")[2];
            if (user.Username == gm)
            {
                string guildId = button.Data.CustomId.Split("-")[1];
                Guild guild = GetGuild(ulong.Parse(guildId));

                string name = button.Data.CustomId.Split("-")[3];

                Quest? quest = guild.GetQuest(name, user);
                if (quest != null)
                {
                    guild.RemoveQuest(quest);
                    await button.UpdateAsync(x =>
                    {
                        x.Content = "Quest creation canceled.";
                        x.Embed = null;
                        x.Components = null;
                    });
                }
                else
                {
                    await button.UpdateAsync(x =>
                    {
                        x.Content = "Somehow Quest was null.";
                    });
                }
            }
            else
            {
                await button.UpdateAsync(x => { });
            }
        }

        private static async Task QuestCreate(SocketMessageComponent selectMenu)
        {
            string gm = selectMenu.Data.CustomId.Split("-")[2];
            IUser user = selectMenu.User;
            if (user.Username == gm)
            {
                string guildId = selectMenu.Data.CustomId.Split("-")[1];
                Guild guild = GetGuild(ulong.Parse(guildId));

                string name = selectMenu.Data.CustomId.Split("-")[3];
                string menuType = selectMenu.Data.CustomId.Split("-")[4];
                string value = string.Join(", ", selectMenu.Data.Values);
                var maxPlayerMenu = Quest.PlayerMaxSelector(guildId, gm, name);
                var messageComponents = new ComponentBuilder();

                Quest? quest = guild.GetQuest(name, user);
                if (quest != null)
                {
                    switch (menuType)
                    {
                        case "questPlayerMax":
                        {
                            quest.SetMaxPlayers(value);
                            messageComponents = Quest.ConfirmationButtons(guildId, gm, name);
                            break;
                        }
                        case "questThreat":
                        {
                            quest.SetThreat(value);
                            messageComponents
                                .WithSelectMenu(maxPlayerMenu)
                                .WithButton(
                                    "Back",
                                    "createQuest-" + guildId + "-" + gm + "-" + name + "-back",
                                    ButtonStyle.Danger
                                );
                            break;
                        }
                    }

                    guild.SaveAll();

                    var questEmbed = quest.GenerateEmbed(user);

                    await selectMenu.UpdateAsync(x =>
                    {
                        x.Content = "Always double-check things are correct before continuing!";
                        x.Embed = questEmbed.Build();
                        x.Components = messageComponents.Build();
                    });
                }
                else
                {
                    await selectMenu.UpdateAsync(x =>
                    {
                        x.Content = "Somehow Quest was null.";
                    });
                }
            }
            else
            {
                await selectMenu.UpdateAsync(x =>
                {
                    x.Content = "User id doesn't match.";
                });
            }
        }

        private static async Task CharacterCreate(
            SocketModal modal,
            List<SocketMessageComponentData> components
        )
        {
            string guildId = modal.Data.CustomId.Split("-")[1];
            Guild guild = GetGuild(ulong.Parse(guildId));
            string playerId = modal.Data.CustomId.Split("-")[2];
            IUser player = modal.User;

            string charName = components.First(x => x.CustomId == "character_name").Value;
            string charDescription = components
                .First(x => x.CustomId == "character_description")
                .Value;
            string charReputation = components
                .First(x => x.CustomId == "character_reputation")
                .Value;

            Dictionary<string, dynamic>? charData;
            IImportable? importedCharacter = guild.GetCharacterJson(ulong.Parse(playerId));
            if (importedCharacter != null)
            {
                charData = importedCharacter.GetCharacterData();
                if (charData != null)
                {
                    Character character =
                        new(player, guild, charName, charDescription, charReputation, charData);
                }
            }

            await modal.RespondAsync(charName + " has been saved.");
        }

        private static async Task QuestCreate(
            SocketModal modal,
            List<SocketMessageComponentData> components
        )
        {
            string gm = modal.Data.CustomId.Split("-")[2];
            IUser gamemaster = modal.User;
            string guildId = modal.Data.CustomId.Split("-")[1];
            Guild guild = GetGuild(ulong.Parse(guildId));

            string name = components.First(x => x.CustomId == "quest_name").Value;
            string description = components.First(x => x.CustomId == "quest_description").Value;
            string allTags = components.First(x => x.CustomId == "quest_tags").Value;
            string[] tags = allTags.Split(
                ",",
                StringSplitOptions.TrimEntries & StringSplitOptions.RemoveEmptyEntries
            );
            Quest quest = new(gamemaster.Id, name, description, new(tags));
            guild.AddQuest(quest);

            var messageComponents = new ComponentBuilder(); //Quest.ConfirmationButtons(guildId, gm, name);
            var threatMenu = Quest.ThreatSelector(guildId, gm, name);
            var maxPlayerMenu = Quest.PlayerMaxSelector(guildId, gm, name);
            messageComponents
                .WithSelectMenu(threatMenu)
                .WithButton(
                    "Cancel",
                    "createQuest-" + guildId + "-" + gm + "-" + name + "-cancel",
                    ButtonStyle.Danger
                );

            var questEmbed = quest.GenerateEmbed(gamemaster);

            await modal.RespondAsync(
                embed: questEmbed.Build(),
                components: messageComponents.Build()
            );
        }

        private static async Task PostQuest(
            SocketMessageComponent context,
            Guild guild,
            Quest quest,
            IUser gm
        )
        {
            guild.SaveAll();
            if (guild.QuestBoard != null)
            {
                EmbedBuilder questEmbed = quest.GenerateEmbed(gm);
                ForumTag[] tags = { guild.QuestBoard.Tags.First(x => x.Name == "Pending") };
                ComponentBuilder components = new ComponentBuilder().WithButton(
                    "Edit",
                    "editQuest-" + guild.Id + "-" + quest.Name + "-editStart",
                    ButtonStyle.Secondary
                );
                IThreadChannel thread = await guild.QuestBoard.CreatePostAsync(
                    quest.Name,
                    ThreadArchiveDuration.OneWeek,
                    embed: questEmbed.Build(),
                    tags: tags,
                    components: components.Build()
                );
                await thread.AddUserAsync((IGuildUser)gm);
                await context.UpdateAsync(x =>
                {
                    x.Content = "Quest posted! " + thread.Mention;
                    x.Embed = null;
                    x.Components = null;
                });
            }
        }

        /* private async Task CharacterCreate(
            SocketModal modal,
            List<SocketMessageComponentData> components
        ) { } */
    }
}
