namespace WingTechBot.Games.GameCommands;

///Lists games that are in progress.
public sealed class ActiveGamesCommand : SlashCommand
{
	public override async Task SetUp(WingTechBot bot)
	{
		var listGamesCommand = new SlashCommandBuilder()
			.WithName("active-games")
			.WithDescription("Lists games that are in progress");

		await AddCommand(bot, listGamesCommand);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		var games = Bot.GameHandler.ActiveGames;

		if (games.Any())
			await command.FollowupAsync($"Active Games:\n{String.Join('\n', games.Select(g => $"* <#{g.ThreadChannel.Id}>"))}");
		else
			await command.FollowupAsync("No active games");
	}
}