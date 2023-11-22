using Discord.Interactions;

namespace Bot {
	public class TestCommand : InteractionModuleBase<SocketInteractionContext> {
		[SlashCommand ("test-slash", "echo an input")]
		public async Task Echo (string input) {
			await RespondAsync (input);
		}
	}
}
