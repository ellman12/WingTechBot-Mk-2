namespace WingTechBot.Games.Hangman;

public sealed class Hangman : Game
{
	public override async Task GameSetup()
	{
		Console.WriteLine("hi");
	}

	protected override Task ProcessMessage(SocketMessage message)
	{
		throw new NotImplementedException();
	}
}