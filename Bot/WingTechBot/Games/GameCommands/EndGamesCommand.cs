namespace WingTechBot.Games.GameCommands;

public sealed class EndGamesCommand : SlashCommand
{
	public override async Task SetUp(WingTechBot bot)
	{
		var endGamesCommand = new SlashCommandBuilder()
			.WithName("end-games")
			.WithDescription("Ends all games");

		await AddCommand(bot, endGamesCommand);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		int count = Bot.GameHandler.ActiveGames.Count;

		if (count == 0)
		{
			await command.FollowupAsync("No games to close");
			return;
		}

		foreach (var game in Bot.GameHandler.ActiveGames)
		{
			Logger.LogLine($"Attempting to close game {game.GetType().Name}");
			await game.EndGame();
		}

		await command.FollowupAsync($"All {count} active games closed");
	}
}