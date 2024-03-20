using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;

namespace Bot.Commands
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _commands = commands;
            _client = client;
        }

        public async Task InstallCommandsAsync()
        {
            // hook MessageReceived event
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            if (messageParam is not SocketUserMessage message)
                return;

            int argPos = 0;

            if (
                !(
                    message.HasStringPrefix("!!", ref argPos)
                    || message.HasMentionPrefix(_client.CurrentUser, ref argPos)
                ) || message.Author.IsBot
            )
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);

            await _commands.ExecuteAsync(context: context, argPos: argPos, services: null);
        }
    }
}
