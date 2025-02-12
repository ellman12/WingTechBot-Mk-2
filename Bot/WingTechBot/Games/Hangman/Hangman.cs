using WingTechBot.Games.Utils;

namespace WingTechBot.Games.Hangman;

public class Hangman : Game
{
	private string _word = string.Empty, _check = string.Empty;
	private readonly HashSet<char> _letterGuesses = [];
	private readonly HashSet<string> _wordGuesses = [];

	private Dictionary<IUser, int> _scores;

	private static readonly Random _random = new();
	private const int StrikeLimit = 6;


	private static readonly string[] _heads =
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
		"(OwO)",
		"(=.=)",
		"(OxO)",
		"(o_o)",
		"(._.)",
		"(;_;)",
		"(^.^)",
		"(~.~)",
	];

	private int _currentHostIndex = -1, _strikes = 0, _score = 0, _total = 0;

	private IUser CurrentHost => Players[_currentHostIndex];

	private bool _pvp;
	private int _clues;
	
	private record RoundFinishState(bool IsWin, IUser Winner, string FinishScreen);

	private static bool IsGuessableCharacter(char c) => char.IsLetter(c) && c.IsAmericanized();
	private static string ReduceToGuessable(string s) => string.Join("", s.Where(IsGuessableCharacter));

	public override async Task GameSetup()
	{
		_pvp = !await UserInput.PromptYN(ThreadChannel, "Would you like to face a bot?", CancelTokenSource.Token);
		_clues = (await UserInput.Prompt<int>(ThreadChannel, "How many clues would you like? (recommended: 0-2)", CancelTokenSource.Token)).Input;

		while (Players.Count < 1 || (_pvp && Players.Count < 2))
		{
			if (Players.Count < 1)
			{
				await SendMessage("You need at least one player.");
				await GetPlayers();
			}

			if (_pvp && Players.Count < 2)
			{
				await SendMessage("You need at least two players to play multiplayer.");
				await GetPlayers();
			}
		}

		if (_pvp)
		{
			_scores = [];

			foreach (var player in Players)
			{
				_scores.Add(player, 0);
			}
		}
	}

	public override async Task RunGame()
	{
		while (true)
		{
			await RoundSetup();
			var finishState = await RunRound();

			if (finishState.IsWin)
			{
				if (_pvp)
				{
					_scores[finishState.Winner]++;
				}
				else
				{
					_score++;
				}
			}

			await SendMessage(finishState.FinishScreen);
			
			string input = await UserInput.StringPrompt(ThreadChannel, "Type \"next\" to continue or \"end\" to stop playing.", CancelTokenSource.Token, s => s.Trim().ToLower() is "next" or "end");

			if (input == "end")
			{
				await Shutdown();
				await SendMessage("Game finished");
				break;
			}
		}
	}

	private async Task RoundSetup()
	{
		_letterGuesses.Clear();
		_wordGuesses.Clear();
		_strikes = 0;

		if (_pvp)
		{
			AdvanceHost();
			await SendMessage($"Prompting {CurrentHost.Username} for next word... Check your DMs");
			var dmChannel = await CurrentHost.CreateDMChannelAsync();
			var receivedWord = await UserInput.StringPrompt(
				dmChannel, 
				"What should the word be?", 
				CancelTokenSource.Token, 
				condition: (string s) => s.Length < 100 
					&& !WordUtils.BannedWords.Contains(s.Trim().ToLower())
			);
			_word = receivedWord.Trim().ToUpper();
		}
		else
		{
			_word = WordUtils.GetRandomWord(
				allowCurses: false, 
				shouldAmericanize: true, 
				minLength: 2, 
				filterFn: word => word.Any(c => "aeiouy".Contains(c))
					&& word.Distinct().Count() >= _clues + 3
			).ToUpper(); // Get random word
		}

		_check = _word.RemoveDiacritics();

		_total++;

		// get clues
		var missingLetters = new HashSet<char>();

		foreach (var c in _check)
		{
			if (IsGuessableCharacter(c))
			{
				missingLetters.Add(c);
			}
		}

		while (_letterGuesses.Count < _clues)
		{
			var c = _word[_random.Next(_word.Length)];
			missingLetters.Remove(c);
			_letterGuesses.Add(c);

			if (missingLetters.Count <= 0)
			{
				throw new Exception($"Hangman crashed while trying to generate clues for word {_word}");
			}
		}

		(ulong id, string text) = (0, null);

		await SendMessage($"===[NEW ROUND]===\n**{WordUtils.GetNumberName(_check.Count(char.IsLetter))}** letters");

		Logger.LogLine($"Starting round of Hangman with a {_word.Length} letter word");

	}
	private async Task<RoundFinishState> RunRound()
	{
		IUser lastUser = null;

		while (true)
		{
			var screen = GetScreen();

			if (_strikes >= StrikeLimit) // strike gameover
			{
				screen += $"Gameover! The word was: {_word}";
				return new RoundFinishState(false, null, screen);
			}
			
			var wordGuessedSuccessfully = _check.All(c => !IsGuessableCharacter(c) || _letterGuesses.Contains(c));

			if (wordGuessedSuccessfully)
			{
				screen += "Correct!";
				if (_pvp)
				{
					screen += $" One point to {lastUser.Username}.";
				}

				return new RoundFinishState(true, lastUser, screen);
			}
		
			await SendMessage(screen);

			UserInput.ReceivedInput<string> input;
			while (true)
			{
				input = await UserInput.Prompt<string>(
					ThreadChannel, 
					"Guess a letter or word!", 
					CancelTokenSource.Token, 
					condition: s => s.Length == 1 
						|| ReduceToGuessable(s).Length == ReduceToGuessable(_check).Length
				);

				lastUser = input.Message.Author;
				if (_pvp && lastUser == CurrentHost)
				{
					await SendMessage("The host cannot guess on their own prompt!");
					continue;
				}

				var guess = input.Input.Trim().ToUpper().RemoveDiacritics();
			
				if (guess.Length == 1 && IsGuessableCharacter(guess[0])) // letter guess
				{
					if (!_letterGuesses.Contains(guess[0]))
					{
						_letterGuesses.Add(guess[0]);
						if (!_check.Contains(guess[0]))
						{
							_strikes++;
						}

						break;
					}	
				}

				if (ReduceToGuessable(guess) == ReduceToGuessable(_check)) // word guess success
				{
					return new RoundFinishState(true, input.Message.Author, "Correct!");
				}
				
				
				if (IsValidGuess(guess, _check, _wordGuesses, _letterGuesses)) // word guess fail
				{
					await SendMessage($"{guess} is not the word.");
					_wordGuesses.Add(guess);
					_strikes++;
				}
			}
		}
	}

	public async Task Shutdown()
	{
		if (_pvp)
		{
			var orderedScores = _scores.OrderByDescending(kvp => kvp.Value);

			var gameOverText = $"Gameover! Scores:\n";

			foreach (var kvp in orderedScores)
			{
				var (player, score) = kvp;
				gameOverText += $"{score} - {player.Username}\n";
			}

			await SendMessage(gameOverText);
		}
		else
		{
			await SendMessage($"Gameover! Got: {_score}/{_total}");
		}
	}

	private string GetScreen()
	{
		var screen = "```" +
					$"||-------\n" +
					$"||      |\n" +
					$"||    {GetHead()}\n" +
					$"||    {GetBody()}\n" +
					$"||    {GetLegs()}\n" +
					$"||\n" +
					$"||\n" +
					$"====[HANGMAN]====\n" +
					$"{GetClue()}\n";

		screen += "```";

		return screen;
	}

	private string GetHead() => _strikes switch
	{
		1 => _heads[0],
		>= 2 => _heads[_random.Next(_heads.Length)],
		_ => "     "
	};

	private string GetBody() => _strikes switch
	{
		2 => "  8  ",
		3 => "  8 -",
		>= 4 => "- 8 -",
		_ => "     "
	};

	private string GetLegs() => _strikes switch
	{
		5 => " /  ",
		6 => @" / \",
		_ => "   "
	};

	private string GetClue()
	{
		var clue = string.Empty;

		for (var i = 0; i < _word.Length; i++)
		{
			if (_letterGuesses.Contains(_check[i]) || !_check[i].IsAmericanized())
			{
				clue += $"{_word[i]} ";
			}
			else
			{
				clue += $"_ ";
			}
		}

		clue += "\n" + "wrong:";

		foreach (var c in _letterGuesses)
		{
			if (!_check.Contains(c))
			{
				clue += $" {c}";
			}
		}

		foreach (var s in _wordGuesses)
		{
			clue += $"\n - {s}";
		}

		return clue;
	}

	private void AdvanceHost()
	{
		_currentHostIndex++;
		if (_currentHostIndex >= Players.Count)
		{
			_currentHostIndex = 0;
		}
	}

	private static bool IsValidGuess(string guess, string word, HashSet<string> previousGuesses, HashSet<char> letterGuesses)
	{
		if (previousGuesses.Contains(guess))
		{
			return false;
		}
	
		if (guess.Length != word.Length)
		{
			return false;
		}
		
		// if letter doesn't match and could not possibly match
		if (guess
			.Select((letter, index) => (letter, index))
			.Any(t => word[t.index] != t.letter 
				&& (letterGuesses.Contains(t.letter) 
					|| letterGuesses.Contains(word[t.index]))))
		{
			return false;
		}
	
		return true;
	}
}