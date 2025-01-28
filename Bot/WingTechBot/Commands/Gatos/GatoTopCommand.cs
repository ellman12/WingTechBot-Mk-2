namespace WingTechBot.Commands.Gatos;

public sealed class GatoTopCommand : SlashCommand
{
	public override async Task SetUp(WingTechBot bot)
	{
		Bot = bot;
		Bot.Client.SlashCommandExecuted += HandleCommand;

		var gatoTopCommand = new SlashCommandBuilder().WithName("gato-top").WithDescription("Shows a leaderboard for how many images each cat has.");

		try
		{
			await Bot.Client.CreateGlobalApplicationCommandAsync(gatoTopCommand.Build());
		}
		catch (Exception e)
		{
			Logger.LogLine("Error adding gato-top command");
			Logger.LogException(e);
		}
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != "gato-top")
			return;

		var gatoLeaderboard = await Gato.GetGatoLeaderboard();

		string message;
		if (gatoLeaderboard.Length > 0)
		{
			var entries = gatoLeaderboard.Aggregate((rankings: new List<(int rank, string name, int count)>(), lastCount: 0, index: 1, lastRank: 0), (current, entry) =>
			{
				var (rankings, lastCount, index, lastRank) = current;
				int newRank = entry.count == lastCount ? lastRank : index;

				rankings.Add((newRank, entry.name, entry.count));
				return (rankings, entry.count, index + 1, newRank);
			}, tuple => tuple.rankings)
			.Select(entry => $"{entry.rank.ToString(),-6} {entry.count.ToString(),-7} {entry.name}");

			message = $"```Gato Leaderboard\nRank   Count   Name\n{String.Join('\n', entries)}```";
		}
		else
		{
			message = "No gatos found";
		}

		await command.FollowupAsync(message);
	}
}