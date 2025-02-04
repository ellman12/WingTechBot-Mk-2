namespace WingTechBot.Games;

///Represents any game running in a <see cref="Task"/> and communicating through a <see cref="SocketThreadChannel"/>.
public abstract class Game
{
	public IUser GameMaster { get; set; }

	///The <see cref="Task"/> this game is running in.
	public Task Task { get; set; }

	///Used to cancel the <see cref="Game.Task"/>, if necessary.
	public CancellationTokenSource CancelTokenSource { get; set; }

	///The Discord Thread this game sends and receives messages in.
	public SocketThreadChannel ThreadChannel { get; set; }

	public IMessage LastMessage { get; private set; }

	public IReadOnlyCollection<SocketGuildUser> AvailablePlayers { get; set; }

	public List<IUser> Players { get; set; } = [];

	public virtual uint MaxPlayers { get; protected set; } = Int32.MaxValue;


	///Any setup this game requires. E.g., number of players, etc.
	public abstract Task GameSetup();

	public abstract Task RunGame();

	public async Task EndGame()
	{
		await CancelTokenSource.CancelAsync();
	}

	public async Task MessageReceived(SocketMessage message)
	{
		LastMessage = message;

		if (!ValidMessage(message))
			return;

		await ProcessMessage(message);
	}

	protected abstract Task ProcessMessage(SocketMessage message);

	protected async Task SendMessage(string message) => await ThreadChannel.SendMessageAsync(message);

	protected async Task GetPlayers()
	{
		while (Players.Count < MaxPlayers)
		{
			if (UserInput.TryGetUser(ThreadChannel, AvailablePlayers, $"Enter player {Players.Count + 1}'s username or \"stop\" to stop adding players", CancelTokenSource.Token, out IUser user))
			{
				Players.Add(user);
				await ThreadChannel.AddUserAsync(user as IGuildUser);
			}
			else
			{
				if (LastMessage.Content.Equals("stop", StringComparison.CurrentCultureIgnoreCase))
					return;

				if (user == null)
					await SendMessage("Player not found");
			}
		}
	}

	private bool ValidMessage(SocketMessage message)
	{
		return message.Channel is SocketThreadChannel && message.Channel.Id == ThreadChannel.Id && !message.Author.IsBot && !message.Author.IsWebhook;
	}
}