namespace WingTechBot.Commands.Reactions;

public sealed class ReactionsCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("reactions")
			.WithDescription("Shows totals for all reactions you have received this year")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("user")
				.WithDescription("An optional user to check the reactions of.")
				.WithType(ApplicationCommandOptionType.User)
				.WithRequired(false)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		await UserReactionsForYear(command);
	}

	private static async Task UserReactionsForYear(SocketSlashCommand command)
	{
		int year = DateTime.Now.Year;
		var user = command.Data.Options.FirstOrDefault(o => o.Name == "user")?.Value as SocketUser ?? command.User;
		var reactions = await Reaction.GetReactionsUserReceived(user.Id, year);

		string message;
		if (reactions.Count > 0)
		{
			message = reactions.Aggregate($"{user.Username} received\n", (current, reaction) => current + $"* {reaction.Value} {reaction.Key}\n");
			message += $"in {year}";
		}
		else
		{
			message = $"No reactions received for {year}";
		}

		await command.FollowupAsync(message);
	}
}