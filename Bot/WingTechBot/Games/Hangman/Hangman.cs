namespace WingTechBot.Games.Hangman;

public sealed class Hangman : Game
{
	private string word = "";
	private readonly List<char> correctLetters = [], incorrectLetters = [];
	private readonly List<string> wordGuesses = [];

	private Dictionary<IUser, int> scores;

	private int currentHostIndex = -1;

	private bool pvp;

	private IUser CurrentHost => Players[currentHostIndex];

	private int Strikes => incorrectLetters.Count + wordGuesses.Count;

	private const int StrikeLimit = 6;

	private static readonly string[] heads =
	[
		"(O.O)",
		"(O.-)",
		"(-.-)",
		"(O.o)",
		"(>.<)",
		"(>.>)",
		"(X.X)",
		"(@.@)",
		"(o-o)",
		"(-_-)",
		"(UwU)",
		"(OwO)", //<-- me_irl when I'm about to die if the player doesn't guess the right word.
		"(=.=)",
		"(OxO)",
		"(o_o)",
		"(._.)",
		"(;_;)",
		"(^.^)",
		"(~.~)"
	];

	public override async Task GameSetup()
	{
		pvp = !await UserInput.PromptYN(ThreadChannel, "Would you like to face a bot? ", CancelTokenSource.Token);

		while (!Players.Any() || (pvp && Players.Count == 1))
		{
			if (!Players.Any())
			{
				await SendMessage("You can't play hangman with zero players!");
				await GetPlayers();
			}

			if (pvp && Players.Count == 1)
			{
				await SendMessage("You need at least two players to play multiplayer.");
				await GetPlayers();
			}
		}

		if (pvp)
		{
			scores = new Dictionary<IUser, int>();

			foreach (var player in Players)
			{
				scores.Add(player, 0);
			}
		}
	}

	public override async Task RunGame()
	{
		while (true)
		{
			await RoundSetup();
			await RunRound();

			string input = await UserInput.Prompt(ThreadChannel, "Type \"next\" to continue or \"end\" to stop playing.", CancelTokenSource.Token, s => s.Trim().ToLower() is "next" or "end");

			if (input == "end")
			{
				await SendMessage("Game finished");
				break;
			}
		}
	}

	private async Task RoundSetup()
	{
		if (pvp)
		{
			AdvanceHost();
			await SendMessage($"Prompting {CurrentHost.Username} for the word...");
			word = (await UserInput.Prompt(await CurrentHost.CreateDMChannelAsync(), "What should the word be?", CancelTokenSource.Token)).Trim().ToUpper();
		}
		else
		{
			word = Words[Random.Shared.Next(0, Words.Length)].Trim().ToUpper();
		}

		correctLetters.Clear();
		incorrectLetters.Clear();
		wordGuesses.Clear();

		await SendMessage(GetScreen());

		Logger.LogLine($"Starting round of Hangman with the word {word}");
	}

	private async Task RunRound()
	{
		while (true)
		{
			string output = "";

			if (Strikes == StrikeLimit)
			{
				await SendMessage($"Game over! The word was: {word}");
				break;
			}

			if (word.All(c => correctLetters.Contains(c)))
			{
				break;
			}

			string input = (await UserInput.Prompt(ThreadChannel, "Guess a letter or word", CancelTokenSource.Token)).ToUpper();

			if (input.Length > 1)
			{
				if (input == word)
				{
					output = "Correct!";
					correctLetters.AddRange(word.Distinct());
				}
				else
				{
					wordGuesses.Add(input);
				}
			}
			else
			{
				char letter = input[0];
				if (word.Contains(letter))
				{
					correctLetters.Add(letter);
					output = "Correct!";
				}
				else
				{
					incorrectLetters.Add(letter);
				}
			}

			output += GetScreen();
			await SendMessage(output);
		}
	}

	private void AdvanceHost()
	{
		currentHostIndex++;
		if (currentHostIndex >= Players.Count)
			currentHostIndex = 0;
	}

	protected override async Task ProcessMessage(SocketMessage message) {}

	private string GetScreen()
	{
		string message =
			$"""
			```
			||-------
			||      |
			||    {Head}
			||    {Body}
			||    {Legs}
			||
			||
			====[HANGMAN]====
			RIGHT: {String.Join(' ', word.Select(c => word.Contains(c) && correctLetters.Contains(c) ? c : '_'))}
			WRONG: {String.Join(' ', incorrectLetters)}
			{String.Join("\n", wordGuesses)}
			```
			""";

		return message;
	}

	private string Head => Strikes switch
	{
		1 => heads[0],
		>= 2 => heads[Random.Shared.Next(heads.Length)],
		_ => "     "
	};

	private string Body => Strikes switch
	{
		2 => "  8  ",
		3 => "  8 -",
		>= 4 => "- 8 -",
		_ => "     "
	};

	private string Legs => Strikes switch
	{
		5 => " /  ",
		6 => @" / \",
		_ => "   "
	};
}