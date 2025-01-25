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
			message = $"```{year} Karma Leaderboard\n";
			for (int i = 0; i < karmaLeaderboard.Length; i++)
			{
				var entry = karmaLeaderboard[i];
				var user = await Bot.Client.GetUserAsync(entry.receiverId);
				message += $"{i + 1}. {user.Username} — {entry.karma}\n";
			}
			message += "```";
		}
		else
		{
			message = $"No karma for {year}";
		}

		await command.FollowupAsync(message);
	}
}