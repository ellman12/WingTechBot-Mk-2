namespace WingTechBot.Commands.Reactions;

public sealed class TopCommand : SlashCommand
{
	public override async Task SetUp(WingTechBot bot)
	{
		var topCommand = new SlashCommandBuilder()
			.WithName("top")
			.WithDescription("Shows the leaderboard for karma");

		await AddCommand(bot, topCommand);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		await GetKarmaLeaderboard(command);
	}

	private async Task GetKarmaLeaderboard(SocketSlashCommand command)
	{
		int year = DateTime.Now.Year;
		var karmaLeaderboard = await Karma.GetKarmaLeaderboard(year);

		if (!karmaLeaderboard.Any())
		{
			await command.FollowupAsync($"No karma for {year}");
			return;
		}

		var entries = karmaLeaderboard.Aggregate(
			(rankings: new List<(int rank, string username, int karma)>(), lastKarma: 0, index: 1, lastRank: 0),
			(current, entry) =>
			{
				var (rankings, lastKarma, index, lastRank) = current;
				int newRank = entry.karma == lastKarma ? lastRank : index;

				var username = Bot.Client.GetUserAsync(entry.receiverId).Result.Username;

				rankings.Add((newRank, username, entry.karma));
				return (rankings, entry.karma, index + 1, newRank);
			},
			result => result.rankings)
			.Select(entry => $"{entry.rank.ToString(),-6} {entry.karma.ToString(),-7} {entry.username}");

		await command.FollowupAsync($"```{year} Karma Leaderboard\n----------------------\nRank   Karma   Name\n{String.Join('\n', entries)}```");
	}
}