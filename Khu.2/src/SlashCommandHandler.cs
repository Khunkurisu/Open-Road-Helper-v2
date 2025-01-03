using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace OpenRoadHelper.Commands
{
    public class SlashCommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _serviceProvider;

        private InteractionService? _interactionService;

        public SlashCommandHandler(DiscordSocketClient client, IServiceProvider serviceProvider)
        {
            _client = client;
            _serviceProvider = serviceProvider;
            _client.Ready += OnClientReady;
        }

        public async Task OnClientReady()
        {
            _interactionService = new InteractionService(_client);
            await _interactionService.AddModulesAsync(
                Assembly.GetEntryAssembly(),
                _serviceProvider
            );
            //await _interactionService.RegisterCommandsGloballyAsync();
            if (Program.RunningLocation.Contains("home"))
            {
                await _interactionService.RegisterCommandsToGuildAsync(1165262696397152276);
            }
            else
            {
                await _interactionService.RegisterCommandsToGuildAsync(1123803241793192046);
            }

            _client.InteractionCreated += async interaction =>
            {
                var scope = _serviceProvider.CreateScope();
                var ctx = new SocketInteractionContext(_client, interaction);
                await _interactionService.ExecuteCommandAsync(ctx, scope.ServiceProvider);
            };
        }
    }
}
