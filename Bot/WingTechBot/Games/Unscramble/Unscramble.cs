using WingTechBot.Games.Utils;

namespace WingTechBot.Games.Unscramble;

public sealed class Unscramble : Game
{
	private string Word { get; set; }

	private string ScrambledWord { get; set; }

	private int attempts = 1;

	public override async Task GameSetup()
	{
		Word = WordUtils.GetRandomWord();
		ScrambledWord = new string([.. Word.OrderBy(_ => Guid.NewGuid())]);

		Logger.LogLine($"Starting a game of Unscramble with the words {Word} and {ScrambledWord}");

		await SendMessage($"The scrambled word is {ScrambledWord}");
	}

	public override async Task RunGame()
	{
		while (!CancelTokenSource.IsCancellationRequested)
		{
			string input = await UserInput.StringPrompt(ThreadChannel, "What is your guess?", CancelTokenSource.Token);

			if (input.Length != Word.Length)
			{
				await SendMessage("Input length is not equal to scrambled word length");
				continue;
			}

			if (string.Equals(input, Word, StringComparison.InvariantCultureIgnoreCase))
			{
				await SendMessage($"Correct! You guessed \"{Word}\" in {attempts} guesses.");
				break;
			}

			await SendMessage("Wrong!");
			attempts++;
		}
	}

	protected override async Task ProcessMessage(SocketMessage message) {}
}
