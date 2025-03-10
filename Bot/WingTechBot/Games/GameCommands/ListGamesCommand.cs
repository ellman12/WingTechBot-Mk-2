namespace WingTechBot.Games.GameCommands;

///Lists available games that can be played.
public sealed class ListGamesCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("list-games")
			.WithDescription("Lists available games that can be played");
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		var games = Bot.GameHandler.AvailableGames;

		if (games.Any())
			await command.FollowupAsync($"Available Games:\n{String.Join('\n', games.Select(g => $"* {g.Name}"))}");
		else
			await command.FollowupAsync("No available games");
	}
}