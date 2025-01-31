namespace WingTechBot.Games.GameCommands;

///Lists available games that can be played.
public sealed class ListGamesCommand : SlashCommand
{
	public override async Task SetUp(WingTechBot bot)
	{
		var listGamesCommand = new SlashCommandBuilder()
			.WithName("list-games")
			.WithDescription("Lists available games that can be played");

		await AddCommand(bot, listGamesCommand);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		var games = Bot.GameHandler.AvailableGames;

		if (games.Any())
			await command.FollowupAsync($"Available Games:\n{String.Join('\n', games.Select(g => $"* {g.Name}"))}");
		else
			await command.FollowupAsync("No available games");
	}
}