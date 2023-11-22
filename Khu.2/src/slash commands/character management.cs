using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Bot.Characters {
	[Group ("character", "Manage your characters with these commands.")]
	public class CharacterManagement : InteractionModuleBase<SocketInteractionContext> {

		[SlashCommand ("create", "Create a character.")]
		public async Task CreateCharacter (string name, string desc, string rep, int age, float height, float weight, int birthday,
			int birthmonth, int birthyear, string ancestry, string heritage, string charClass, string color, string avatar, string sheet) {
			await RespondAsync (name);
			//ulong messageId = SetToNewMessageID();
			SocketUser user = Context.Client.CurrentUser;
			await ReplyAsync ($"{user.Username}#{user.Discriminator}");
		}

		[SlashCommand ("update", "Update a character.")]
		public async Task UpdateCharacter (string name, string desc = "", string rep = "", int age = 0, float height = 0f, float weight = 0f,
			int birthday = 0, int birthmonth = 0, int birthyear = 0, string ancestry = "", string heritage = "", string charClass = "",
			string color = "", string avatar = "", string sheet = "") {
			await RespondAsync (name);
			//ulong messageId = SetToNewMessageID();
			SocketUser user = Context.Client.CurrentUser;
			await ReplyAsync ($"{user.Username}#{user.Discriminator}");
		}
	}

	public class CharacterNameSuggestions : AutocompleteHandler {
		public override async Task<AutocompletionResult> GenerateSuggestionsAsync (IInteractionContext context,
			IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services) {
			IEnumerable<AutocompleteResult> results = new [] {
				new AutocompleteResult ("name1", "value1"),
					new AutocompleteResult ("name2", "value2")
			};
			return AutocompletionResult.FromSuccess (results.Take (25));
		}
	}
}
