namespace WingTechBot.Games;

///Represents any game running in a <see cref="SocketThreadChannel"/>.
public abstract class Game
{
	public IUser GameMaster { get; set; }

	public SocketThreadChannel ThreadChannel { get; set; }

	public List<IUser> Players { get; set; }

	public virtual uint? MaxPlayers { get; protected set; } = null;
	///List of words that can be used for games like <see cref="Hangman"/>
	protected string[] Words { get; } = File.ReadAllLines(Path.Combine(Program.ProjectRoot, "Games", "words.txt"));

	///Any setup this game requires. E.g., number of players, etc.
	protected abstract Task GameSetup();
}