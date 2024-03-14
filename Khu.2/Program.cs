using Bot.Commands;
using Bot.Quests;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Bot
{
    public class Program
    {
        public static readonly Dictionary<string, GMAvailability> GMAvailabilities = new();

        public static Task Main(string[] args) => new Program().MainAsync();

        public Program()
        {
            _commands = new CommandService();
            _serviceProvider = CreateProvider();
        }

        private readonly CommandService _commands;
        private CommandHandler? _commandHandler;
        private LoggingService? _logging;
        private readonly IServiceProvider _serviceProvider;

        private List<SlashCommandHandler> _slashCommandHandlers = new();

        static IServiceProvider CreateProvider()
        {
            var config = new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 100,
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
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
            _commandHandler = new CommandHandler(_client, _commands);
            _logging = new LoggingService(_client, _commands);

            _slashCommandHandlers.Add(new SlashCommandHandler(_client, _serviceProvider));

            // login asynchronously to discord with client as bot using token
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await _commandHandler.InstallCommandsAsync();

            // block this task until the program closes
            await Task.Delay(-1);
        }
    }
}
