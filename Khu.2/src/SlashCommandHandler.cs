using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

public class SlashCommandHandler {

	private readonly DiscordSocketClient _client;
	private readonly IServiceProvider _serviceProvider;

	private InteractionService? _interactionService;

	public SlashCommandHandler (DiscordSocketClient client, IServiceProvider serviceProvider) {
		_client = client;
		_serviceProvider = serviceProvider;
		_client.Ready += Client_Ready;
	}

	public async Task Client_Ready () {
		_interactionService = new InteractionService (_client);
		await _interactionService.AddModulesAsync (Assembly.GetEntryAssembly (), _serviceProvider);
		await _interactionService.RegisterCommandsGloballyAsync ();

		_client.InteractionCreated += async interaction => {
			var scope = _serviceProvider.CreateScope ();
			var ctx = new SocketInteractionContext (_client, interaction);
			await _interactionService.ExecuteCommandAsync (ctx, scope.ServiceProvider);
		};
	}
}
