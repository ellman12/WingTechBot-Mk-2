namespace WingTechBot.Games;

public sealed class GameHandler
{
	public WingTechBot Bot { get; private init; }

	public Type[] AvailableGames { get; private init; }

	public List<Game> ActiveGames { get; } = [];

	public GameHandler(WingTechBot bot)
	{
		Bot = bot;

		var gameType = typeof(Game);
		AvailableGames = gameType.Assembly.GetTypes().Where(t => t.IsSubclassOf(gameType)).ToArray();
	}

	public async Task CreateGame(SocketSlashCommand command, string gameName)
	{
		var game = (Game)Activator.CreateInstance(AvailableGames.First(g => String.Equals(g.Name, gameName, StringComparison.InvariantCultureIgnoreCase)));
		if (game == null)
		{
			await Logger.LogExceptionAsMessage(new Exception($"Error creating game {gameName}"), command.Channel);
			return;
		}

		game.ThreadChannel = await Bot.BotChannel.CreateThreadAsync(gameName, autoArchiveDuration: ThreadArchiveDuration.OneHour);
		await command.FollowupAsync($"Game started: <#{game.ThreadChannel.Id}>");
		ActiveGames.Add(game);
	}

}