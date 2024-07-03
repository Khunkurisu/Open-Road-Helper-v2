using Bot.Guilds;
using Bot.Helpers;
using Discord;

namespace Bot.Characters
{
    public partial class Character
    {
        private Color GetDiscordColor()
        {
            int[] colorValues = GenericHelpers.HexStringToRGB(_colorPref);
            return new Color(colorValues[0], colorValues[1], colorValues[2]);
        }

        public EmbedBuilder GenerateDetailsEmbed(IUser player)
        {
            Guild guild = Manager.GetGuild(_guild);

            var detailsEmbed = new EmbedBuilder()
                .WithTitle($"{Heritage} {Ancestry} {Class} {Level}")
                .WithAuthor(Name)
                .WithDescription(Description)
                .WithColor(GetDiscordColor())
                .WithFooter(player.Username)
                .AddField("Reputation", Reputation, false)
                .AddField("Age", Age, true)
                .AddField("Height", Height + " cm", true)
                .AddField("Weight", Weight + " kg", true)
                .AddField("Gender", Gender, true)
                .AddField("PT", guild.GetPlayerTokenCount(User).ToString())
                .AddField("DT", Downtime.ToString(), true)
                .AddField("Coin", Gold + " gp", true)
                .WithImageUrl(Avatars.Count > 0 ? Avatars[0] : string.Empty);

            return detailsEmbed;
        }

        public EmbedBuilder GenerateAttributesEmbed(IUser player)
        {
            var attributesEmbed = new EmbedBuilder()
                .WithTitle(Heritage + " " + Ancestry + " " + Class + " " + Level)
                .WithAuthor(Name)
                .WithColor(GetDiscordColor())
                .WithFooter(player.Username);

            List<string> attributes = new();
            foreach (string k in Attributes.Keys)
            {
                attributes.Add(
                    "**"
                        + StringExtensions.FirstCharToUpper(k)
                        + "** "
                        + GenericHelpers.ModifierToString(Modifiers[k])
                );
            }
            attributesEmbed.AddField("Attributes", string.Join(", ", attributes));

            List<string> saves = new();
            foreach (string k in Saves.Keys)
            {
                saves.Add(
                    "**"
                        + StringExtensions.FirstCharToUpper(k)
                        + "** "
                        + GenericHelpers.ModifierToString((int)Saves[k])
                );
            }
            attributesEmbed.AddField("Saves", string.Join(", ", saves));
            attributesEmbed.AddField(
                "Perception",
                GenericHelpers.ModifierToString((int)Perception)
            );

            List<string> skills = new();
            foreach (string k in Skills.Keys)
            {
                skills.Add(
                    StringExtensions.FirstCharToUpper(k)
                        + " "
                        + GenericHelpers.ModifierToString((int)Skills[k])
                );
            }
            string skillField = "**Skills** " + string.Join(", ", skills);
            if (Lore.Count > 0)
            {
                List<string> lore = new();
                foreach (string k in Lore.Keys)
                {
                    lore.Add(
                        StringExtensions.FirstCharToUpper(k)
                            + " "
                            + GenericHelpers.ModifierToString((int)Lore[k])
                    );
                }
                skillField += "; **Lore** " + string.Join(" ,", lore);
            }
            attributesEmbed.AddField("Training", skillField);

            return attributesEmbed;
        }

        public EmbedBuilder GenerateEquipmentEmbed(IUser player)
        {
            return new EmbedBuilder()
                .WithDescription("Work In Progress")
                .WithTitle(Heritage + " " + Ancestry + " " + Class + " " + Level)
                .WithAuthor(Name)
                .WithColor(GetDiscordColor())
                .WithFooter(player.Username);
        }

        public EmbedBuilder GenerateSpellsEmbed(IUser player)
        {
            return new EmbedBuilder()
                .WithDescription("Work In Progress")
                .WithTitle(Heritage + " " + Ancestry + " " + Class + " " + Level)
                .WithAuthor(Name)
                .WithColor(GetDiscordColor())
                .WithFooter(player.Username);
        }

        public EmbedBuilder GenerateFeatsEmbed(IUser player)
        {
            return new EmbedBuilder()
                .WithDescription("Work In Progress")
                .WithTitle(Heritage + " " + Ancestry + " " + Class + " " + Level)
                .WithAuthor(Name)
                .WithColor(GetDiscordColor())
                .WithFooter(player.Username);
        }

        public EmbedBuilder GenerateEmbed(IUser player)
        {
            return DisplayMode switch
            {
                Display.Details => GenerateDetailsEmbed(player),
                Display.Attributes => GenerateAttributesEmbed(player),
                Display.Equipment => GenerateEquipmentEmbed(player),
                Display.Spells => GenerateSpellsEmbed(player),
                Display.Feats => GenerateFeatsEmbed(player),
                _ => new(),
            };
        }

        public EmbedBuilder GenerateEmbed()
        {
            return DisplayMode switch
            {
                Display.Details => GenerateDetailsEmbed(),
                Display.Attributes => GenerateAttributesEmbed(),
                Display.Equipment => GenerateEquipmentEmbed(),
                Display.Spells => GenerateSpellsEmbed(),
                Display.Feats => GenerateFeatsEmbed(),
                _ => new(),
            };
        }

        public EmbedBuilder GenerateDetailsEmbed()
        {
            IUser? player = Manager.GetGuildUser(_guild, _user);
            if (player == null)
            {
                return new EmbedBuilder();
            }
            return GenerateDetailsEmbed(player);
        }

        public EmbedBuilder GenerateAttributesEmbed()
        {
            IUser? player = Manager.GetGuildUser(_guild, _user);
            if (player == null)
            {
                return new EmbedBuilder();
            }
            return GenerateAttributesEmbed(player);
        }

        public EmbedBuilder GenerateEquipmentEmbed()
        {
            IUser? player = Manager.GetGuildUser(_guild, _user);
            if (player == null)
            {
                return new EmbedBuilder();
            }
            return GenerateEquipmentEmbed(player);
        }

        public EmbedBuilder GenerateSpellsEmbed()
        {
            IUser? player = Manager.GetGuildUser(_guild, _user);
            if (player == null)
            {
                return new EmbedBuilder();
            }
            return GenerateSpellsEmbed(player);
        }

        public EmbedBuilder GenerateFeatsEmbed()
        {
            IUser? player = Manager.GetGuildUser(_guild, _user);
            if (player == null)
            {
                return new EmbedBuilder();
            }
            return GenerateFeatsEmbed(player);
        }
    }
}
