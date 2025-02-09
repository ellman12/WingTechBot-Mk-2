namespace WingTechBot.Commands.Reactions;

public sealed class TopMessagesCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("top-messages")
			.WithDescription("Returns a selection of your messages that got the most reactions with this emote")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("emote-name")
				.WithDescription("The name of the emote")
				.WithType(ApplicationCommandOptionType.String)
				.WithRequired(false)
			)
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("amount")
				.WithDescription("How many messages")
				.WithType(ApplicationCommandOptionType.Integer)
				.WithRequired(false)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		await GetTopMessages(command);
	}

	private async Task GetTopMessages(SocketSlashCommand command)
	{
		var options = command.Data.Options;

		var emoteName = options.FirstOrDefault(o => o.Name == "emote-name")?.Value?.ToString()?.ToLower() ?? "upvote";
		int amount = options.FirstOrDefault(o => o.Name == "amount")?.Value as int? ?? 5;

		var messages = await Reaction.GetTopMessagesForUser(command.User.Id, emoteName, amount);

		if (!messages.Any())
		{
			await command.FollowupAsync($"No messages with \"{emoteName}\" found");
			return;
		}

		var entries = messages.Select(entry => $"{entry.count.ToString(),-4} {command.Channel.GetMessageAsync(entry.messageId).Result.GetJumpUrl()}");
		await command.FollowupAsync($"Top messages for {messages.First().emote}\n\n{String.Join('\n', entries)}");
	}
}