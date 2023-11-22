using Discord.Interactions;

[Group("quest", "Manage quests with these commands.")]
public class QuestManagement : InteractionModuleBase<SocketInteractionContext> {

	[SlashCommand ("create", "Create a quest.")]
	public async Task CreateQuest (string input) {
		await RespondAsync (input);
	}
}
