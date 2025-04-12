namespace WingTechBot.Commands.Reactions;

public sealed class TopEmotesCommand : SlashCommand
{
	public override bool Defer => false;

	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("top-emotes")
			.WithDescription("Totals up how many reactions of each emote have been sent this year (including legacy karma)");
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		await TopEmotesForYear(command);
	}

	private async Task TopEmotesForYear(SocketSlashCommand command)
	{
		int year = DateTime.Now.Year;
		var emoteLeaderboard = await Reaction.GetEmoteLeaderboardForYear(year);

		if (!emoteLeaderboard.Any())
		{
			await command.RespondAsync($"No reactions for {year}", ephemeral: true);
			return;
		}

		var entries = emoteLeaderboard.Aggregate(
			(rankings: new List<(int rank, string name, int count)>(), lastCount: null as int?, index: 1, lastRank: 0),
			(current, entry) =>
			{
				var (rankings, lastCount, index, lastRank) = current;
				int newRank = entry.Value == lastCount ? lastRank : index;

				rankings.Add((newRank, entry.Key.Name, entry.Value));
				return (rankings, entry.Value, index + 1, newRank);
			},
			result => result.rankings)
			.Select(entry => $"{entry.rank.ToString(),-6} {entry.count.ToString(),-7} {entry.name}");

		await command.RespondAsync($"```{year} Emote Leaderboard\n----------------------\nRank   Count   Emote\n{String.Join('\n', entries)}```", ephemeral: true);
	}
}