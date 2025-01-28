namespace WingTechBot.Commands.Gatos;

public sealed class GatoTopCommand : SlashCommand
{
	public override async Task SetUp(WingTechBot bot)
	{
		var gatoTopCommand = new SlashCommandBuilder().WithName("gato-top").WithDescription("Shows a leaderboard for how many images each cat has.");
		await AddCommand(bot, gatoTopCommand);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		var gatoLeaderboard = await Gato.GetGatoLeaderboard();

		if (!gatoLeaderboard.Any())
		{
			await command.FollowupAsync("No gatos found");
			return;
		}

		var entries = gatoLeaderboard.Aggregate(
			(rankings: new List<(int rank, string name, int count)>(), lastCount: 0, index: 1, lastRank: 0),
			(current, entry) =>
			{
				var (rankings, lastCount, index, lastRank) = current;
				int newRank = entry.count == lastCount ? lastRank : index;

				rankings.Add((newRank, entry.name, entry.count));
				return (rankings, entry.count, index + 1, newRank);
			},
			result => result.rankings)
			.Select(entry => $"{entry.rank.ToString(),-6} {entry.count.ToString(),-7} {entry.name}");

		await command.FollowupAsync($"```Gato Leaderboard\n----------------\nRank   Count   Name\n{String.Join('\n', entries)}```");
	}
}