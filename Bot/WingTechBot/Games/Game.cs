namespace WingTechBot.Games;

///Represents any game running in a <see cref="Task"/> and communicating through a <see cref="SocketThreadChannel"/>.
public abstract class Game
{
	public IUser GameMaster { get; set; }

	///The <see cref="Task"/> this game is running in.
	public Task Task { get; set; }

	///Used to cancel the <see cref="Game.Task"/>, if necessary.
	public CancellationTokenSource CancelToken { get; set; }

	///The Discord Thread this game sends and receives messages in.
	public SocketThreadChannel ThreadChannel { get; set; }

	public IMessage LastMessage { get; private set; }

	public List<IUser> Players { get; set; }

	public virtual uint MaxPlayers { get; protected set; } = Int32.MaxValue;

	///List of words that can be used for games like <see cref="Hangman"/>
	protected string[] Words { get; } = File.ReadAllLines(Path.Combine(Program.ProjectRoot, "Games", "words.txt"));

	///Any setup this game requires. E.g., number of players, etc.
	public abstract Task GameSetup();

	public  async Task EndGame()
	{
		await CancelToken.CancelAsync();
	}

	public async Task MessageReceived(SocketMessage message)
	{
		LastMessage = message;

		if (!ValidMessage(message))
			return;

		await ProcessMessage(message);
	}

	protected abstract Task ProcessMessage(SocketMessage message);

	private bool ValidMessage(SocketMessage message)
	{
		return message.Channel is SocketThreadChannel && message.Channel.Id == ThreadChannel.Id && !message.Author.IsBot && !message.Author.IsWebhook;
	}

}
