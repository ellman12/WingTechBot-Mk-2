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

		game.ThreadChannel = await Bot.BotChannel.CreateThreadAsync($"{gameName} {DateTime.Now:g}", autoArchiveDuration: ThreadArchiveDuration.OneHour);
		ActiveGames.Add(game);

		Bot.Client.MessageReceived += game.MessageReceived;

		game.CancelToken = new CancellationTokenSource();
		game.Task = Task.Run(async Task () =>
		{
			try
			{
				await game.GameSetup();
			}
			catch (Exception e)
			{
				if (e is not TaskCanceledException)
					await Logger.LogExceptionAsMessage(e, Bot.BotChannel);
			}
		}, game.CancelToken.Token);

		await command.FollowupAsync($"Game started: <#{game.ThreadChannel.Id}>");
	}
}