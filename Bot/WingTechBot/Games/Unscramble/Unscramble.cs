namespace WingTechBot.Games.Unscramble;

public sealed class Unscramble : Game
{
	private string Word { get; set; }

	private string ScrambledWord { get; set; }

	public override async Task GameSetup()
	{
		Word = Words[Random.Shared.Next(0, Words.Length)];
		ScrambledWord = new string(Word.OrderBy(_ => Guid.NewGuid()).ToArray());
		Logger.LogLine($"Starting a game of Unscramble with the words {Word} and {ScrambledWord}");

		await UserInput.Prompt<string>(ThreadChannel, "Test", CancelToken.Token);
	}

	protected override async Task ProcessMessage(SocketMessage message)
	{

	}
}