namespace WingTechBot.Commands.Reactions;

public sealed class ReactionsCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("reactions")
			.WithDescription("Shows totals for all reactions you have received this year");
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		var options = command.Data.Options;

		if (options.Count == 0)
		{
			await UserReactionsForYear(command);
			return;
		}
	}

	private static async Task UserReactionsForYear(SocketSlashCommand command)
	{
		int year = DateTime.Now.Year;
		var reactions = await Reaction.GetReactionsUserReceived(command.User.Id, year);

		string message;
		if (reactions.Count > 0)
		{
			message = reactions.Aggregate($"{command.User.Username} received\n", (current, reaction) => current + $"* {reaction.Value} {reaction.Key}\n");
			message += $"in {year}";
		}
		else
		{
			message = $"No reactions received for {year}";
		}

		await command.FollowupAsync(message);
	}
}