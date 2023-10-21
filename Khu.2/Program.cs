using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class Program {
	public static Task Main (string[] args) => new Program ().MainAsync ();

	private DiscordSocketClient _client;
	private CommandService _commands;
	private CommandHandler _commandHandler;
	private LoggingService _logging;

	public async Task MainAsync () {
		// create client and assign log method, create command handlers
		_client = new DiscordSocketClient ();
		_commands = new CommandService ();
		_commandHandler = new CommandHandler (_client, _commands);
		_logging = new LoggingService (_client, _commands);

		// assign token from file (token.txt added to .gitignore for security)
		var token = File.ReadAllText ("token.txt");

		// login asynchronously to discord with client as bot using token
		await _client.LoginAsync (TokenType.Bot, token);
		await _client.StartAsync ();

		await _commandHandler.InstallCommandsAsync ();

		// block this task until the program closes
		await Task.Delay (-1);
	}
}