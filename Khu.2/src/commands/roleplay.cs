// Create a module with the 'sample' prefix
using Discord.Commands;

public class Roleplay : ModuleBase<SocketCommandContext> {

	// ~sample square 20 -> 400
	[Command ("rp")]
	[Summary ("Make a text post as a given character.")]
	public async Task RPAsync (
		[Summary ("The character to post as.")] string name,
		[Summary ("The text to post.")] string msg) {
		// We can also access the channel from the Command Context.
		await Context.Channel.SendMessageAsync ($"## {name}\n{msg}");
	}
}
