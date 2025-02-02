namespace WingTechBot.Games.Hangman;

public sealed class Hangman : Game
{
	public override async Task GameSetup()
	{
		Console.WriteLine("hi");
	}

	public override async Task RunGame()
	{
		throw new NotImplementedException();
	}

	protected override Task ProcessMessage(SocketMessage message)
	{
		throw new NotImplementedException();
	}
}