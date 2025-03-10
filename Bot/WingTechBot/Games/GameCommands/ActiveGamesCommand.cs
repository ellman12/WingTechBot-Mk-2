namespace WingTechBot.Games.GameCommands;

///Lists games that are in progress.
public sealed class ActiveGamesCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("active-games")
			.WithDescription("Lists games that are in progress");
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		var games = Bot.GameHandler.ActiveGames;

		if (games.Any())
			await command.FollowupAsync($"Active Games:\n{String.Join('\n', games.Select(g => $"* <#{g.ThreadChannel.Id}>"))}");
		else
			await command.FollowupAsync("No active games");
	}
}