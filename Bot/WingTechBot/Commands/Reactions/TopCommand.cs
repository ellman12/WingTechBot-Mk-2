namespace WingTechBot.Commands.Reactions;

public sealed class TopCommand : SlashCommand
{
	public override async Task SetUp(WingTechBot bot)
	{
		Bot = bot;
		Bot.Client.SlashCommandExecuted += HandleCommand;

		var topCommand = new SlashCommandBuilder()
			.WithName("top")
			.WithDescription("Shows the leaderboard for karma");

		try
		{
			await Bot.Client.CreateGlobalApplicationCommandAsync(topCommand.Build());
		}
		catch (Exception e)
		{
			Logger.LogLine("Error adding reactions command");
			Logger.LogException(e);
		}
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != "top")
			return;

		await GetKarmaLeaderboard(command);
	}

	private async Task GetKarmaLeaderboard(SocketSlashCommand command)
	{
		int year = DateTime.Now.Year;
		var karmaLeaderboard = await Karma.GetKarmaLeaderboard(year);

		string message;
		if (karmaLeaderboard.Length > 0)
		{
			int rank = 0;
			message = karmaLeaderboard.Select((entry, index) =>
			{
				if (index == 0 || entry.karma != karmaLeaderboard[index - 1].karma)
					rank = index + 1;

				var username = Bot.Client.GetUserAsync(entry.receiverId).Result.Username;
				return (rank, username, entry.karma);
			})
			.Aggregate($"```{year} Karma Leaderboard\n", (current, entry) => current + $"{entry.rank}. {entry.karma.ToString().PadLeft(3),-4} {entry.username}\n");
			message += "```";
		}
		else
		{
			message = $"No karma for {year}";
		}

		await command.FollowupAsync(message);
	}
}