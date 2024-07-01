using Bot.Guilds;
using Bot.Helpers;
using Bot.PF2;
using Discord;

namespace Bot.Characters
{
    public partial class Character
    {
        public EmbedBuilder? GenerateDetailsEmbed()
        {
            IUser? player = BotManager.GetGuildUser(_guild, _user);
            if (player == null)
            {
                return null;
            }

            return GenerateDetailsEmbed(player);
        }

        public EmbedBuilder? GenerateAttributesEmbed()
        {
            IUser? player = BotManager.GetGuildUser(_guild, _user);
            if (player == null)
            {
                return null;
            }

            return GenerateAttributesEmbed(player);
        }

        public EmbedBuilder? GenerateEquipmentEmbed()
        {
            IUser? player = BotManager.GetGuildUser(_guild, _user);
            if (player == null)
            {
                return null;
            }

            return GenerateEquipmentEmbed(player);
        }

        public EmbedBuilder? GenerateSpellsEmbed()
        {
            IUser? player = BotManager.GetGuildUser(_guild, _user);
            if (player == null)
            {
                return null;
            }

            return GenerateSpellsEmbed(player);
        }

        public EmbedBuilder? GenerateFeatsEmbed()
        {
            IUser? player = BotManager.GetGuildUser(_guild, _user);
            if (player == null)
            {
                return null;
            }

            return GenerateFeatsEmbed(player);
        }

        public EmbedBuilder? GenerateEmbed()
        {
            IUser? player = BotManager.GetGuildUser(_guild, _user);
            if (player == null)
            {
                return null;
            }

            return GenerateEmbed(player);
        }
    }
}
