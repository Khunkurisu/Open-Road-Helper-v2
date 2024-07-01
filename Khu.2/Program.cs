using Bot.Commands;
using Bot;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Bot
{
    public class Program
    {
        public static Task Main(string[] args) => new Program().MainAsync();

        public BotManager? Bot;

        public Program()
        {
            _commands = new CommandService();
            _serviceProvider = CreateProvider();
        }

        private readonly CommandService _commands;
        private LoggingService? _logging;
        private readonly IServiceProvider _serviceProvider;
        private SlashCommandHandler? _slashCommandHandler;

        static IServiceProvider CreateProvider()
        {
            var config = new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 100,
                UseInteractionSnowflakeDate = false,
                GatewayIntents =
                    GatewayIntents.AllUnprivileged
                    | GatewayIntents.MessageContent
                    | GatewayIntents.GuildMembers
            };
            var collection = new ServiceCollection()
                .AddSingleton(config)
                .AddSingleton<DiscordSocketClient>();
            return collection.BuildServiceProvider();
        }

        public async Task MainAsync()
        {
            // assign token from file (token.txt added to .gitignore for security)
            var token = File.ReadAllText("token.txt");

            var _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
            Bot = new(_client);
            _logging = new LoggingService(_client, _commands);

            _slashCommandHandler = new SlashCommandHandler(_client, _serviceProvider);

            // login asynchronously to discord with client as bot using token
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // block this task until the program closes
            await Task.Delay(-1);
        }
    }
}
