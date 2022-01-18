namespace WingTechBot.Hangman;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Discord;

#pragma warning disable
using Game = WingTechBot.Game; // the fact this can be simplified to "using Game = Game;" is terrifying
#pragma warning restore

public class Hangman : Game
{
	private string _word = string.Empty, _check = string.Empty;
	private readonly List<char> _guesses = new();
	private readonly List<string> _wordGuesses = new();

	private Dictionary<ulong, int> _scores;

	private static HashSet<string> _banned;

	private const string DICTIONARY_PATH = @"Hangman\words.txt";
	private const string BANNED_PATH = @"Hangman\banned.txt";

	private static readonly Random _random = new();

	private static readonly string[] _heads = new string[]
	{
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
	};

	protected override Dictionary<string, Action<IMessage, string[]>> Commands => _commands;

	private Dictionary<string, Action<IMessage, string[]>> _commands;
	private int _currentHostIndex = -1, _strikes = 0, _score = 0, _total = 0;

	private bool _pvp;
	private int _clues;
	private static int? _dictionaryCount = null;

	protected override bool Debug => false;

	protected override void Start()
	{
		_commands = new Dictionary<string, Action<IMessage, string[]>>
			{
				{ "CORRECT", CorrectSpelling }
			};

		_pvp = !PromptYN(GamemasterID, AllowedChannels, true, "Would you like to face a bot? (y/n)");
		_clues = Prompt(GamemasterID, AllowedChannels, (int x) => x >= 0, true, "How many clues would you like? (recommended: 0-2)");
	}

	public override void RunGame()
	{
		while (PlayerIDs.Count == 0 || (_pvp && PlayerIDs.Count == 1))
		{
			if (PlayerIDs.Count == 0)
			{
				WriteLine("You can't play hangman with zero players!");
				GetPlayers();
			}

			if (_pvp)
			{
				if (PlayerIDs.Count == 1)
				{
					WriteLine("You need at least two players to play multiplayer!");
					GetPlayers();
				}
			}
		}

		if (_pvp)
		{
			_scores = new Dictionary<ulong, int>();

			foreach (ulong id in PlayerIDs) _scores.Add(id, 0);
		}
		else InitializeDictionary();

		while (true)
		{
			if (_pvp)
			{
				AdvanceHost();
				WriteLine($"Prompting {GetPlayer(PlayerIDs[_currentHostIndex]).Mention} for next word... Check your DMs");
				_word = Prompt(PlayerIDs[_currentHostIndex], PromptMode.DM, (string s) => s.Length < 100, false, "What should the word be?", true).Trim().ToUpper();
			}
			else _word = GetRandomWord().ToUpper(); // Get random word
			_check = _word.RemoveDiacritics();

			WriteLine($"===[NEW ROUND]===\n**{GetWord(_check.Count(c => char.IsLetter(c) && c.IsAmericanized()))}** letters");

			_total++;

			// get clues
			List<char> letters = new();
			int actualClues = _clues;

			if (_pvp)
			{
				foreach (char c in _check) if (!letters.Contains(c) && c.IsAmericanized() && (_clues > 6 || "aeiouyAEIOUY".Contains(c))) letters.Add(c);
			}
			else
			{
				foreach (char c in _check) if (!letters.Contains(c) && c.IsAmericanized()) letters.Add(c);
				actualClues = letters.Count - _clues <= 2 ? letters.Count - 2 : _clues;
			}

			for (int i = 0; i < actualClues; i++)
			{
				char c = letters[_random.Next(letters.Count)];
				letters.Remove(c);
				_guesses.Add(c);

				if (letters.Count <= 0) break;
			}

			(ulong id, string text) tuple = (0, null);

			// main loop
			while (true)
			{
				DeleteSavedMessages();
				string screen = GetScreen();

				if (_strikes >= 6) // strike gameover
				{
					screen += $"Gameover! The word was: {_word}";
					SaveWriteLine(screen);
					break;
				}
				else // win gameover
				{
					bool done = true;
					foreach (char c in _check)
					{
						if (char.IsLetter(c) && c.IsAmericanized()) done &= _guesses.Contains(c);
					}

					if (done)
					{
						screen += "Correct!";
						if (_scores is null) _score++;
						else _scores[tuple.id]++;

						SaveWriteLine(screen);
						break;
					}
					else
					{
						screen += "Guess a letter!";
						SaveWriteLine(screen);
					}
				}

				do
				{
					tuple = PromptAny(AllowedChannels, (string s) => s.Length == 1 || s.Length == _word.Length, true, saveMessage: true);
				}
				while (_currentHostIndex != -1 && PlayerIDs[_currentHostIndex] == tuple.id);

				string guess = tuple.text.Trim().ToUpper().RemoveDiacritics();

				if (guess == _check) // word guess success
				{
					SaveWriteLine("Correct!");

					if (_scores is null) _score++;
					else _scores[tuple.id]++;
					break;
				}
				else if (guess.Length == 1 && char.IsLetter(guess[0])) // letter guess
				{
					if (!_guesses.Contains(guess[0]))
					{
						_guesses.Add(guess[0]);
						if (!_check.Contains(guess[0])) _strikes++;
					}
				}
				else if (!_wordGuesses.Contains(guess)) // word guess fail
				{
					SaveWriteLine($"{guess} is not the word.");
					_wordGuesses.Add(guess);
					_strikes++;
				}
			}

			_strikes = 0;
			_guesses.Clear();
			sentMessages.Clear();
			receivedMessages.Clear();
			_wordGuesses.Clear();

			if (PromptEnd()) break;
		}

		DeleteSavedMessages();

		Shutdown();
	}

	public override void Shutdown()
	{
		if (_pvp)
		{
			var orderedScores = from kvp in _scores orderby kvp.Value descending select kvp;

			string gameOverText = $"Gameover! Scores:\n";

			foreach (var kvp in orderedScores)
			{
				IUser player = GetPlayer(kvp.Key);
				gameOverText += $"{kvp.Value} - {player.Username}#{player.Discriminator}\n";
			}

			WriteLine(gameOverText);
		}
		else WriteLine($"Gameover! Got: {_score}/{_total}");
	}

	private string GetScreen()
	{
		string screen = "```" +
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
		string clue = string.Empty;

		for (int i = 0; i < _word.Length; i++)
		{
			if (_guesses.Contains(_check[i]) || !_check[i].IsAmericanized()) clue += $"{_word[i]} ";
			else clue += $"_ ";
		}

		clue += "\n" + "wrong:";

		foreach (char c in _guesses)
		{
			if (!_check.Contains(c)) clue += $" {c}";
		}

		foreach (string s in _wordGuesses)
		{
			clue += $"\n - {s}";
		}

		return clue;
	}

	private static void InitializeDictionary()
	{
		if (_banned is null)
		{
			_banned = new HashSet<string>();
			FileInfo fi = new(BANNED_PATH);

			using StreamReader file = new(fi.Open(FileMode.Open));
			while (!file.EndOfStream) _banned.Add(file.ReadLine());
		}
	}

	private static string GetRandomWord()
	{
		string word;

		do
		{
			var lines = File.ReadLines(DICTIONARY_PATH);

			_dictionaryCount ??= lines.Count();

			word = lines.Skip(_random.Next((int)_dictionaryCount - 1)).Take(1).First();
		}
		while (_banned.Contains(word) || word.Length <= 2 || !word.Any((char c) => "aeiouy".Contains(c)) || !word.IsAmericanized());

		return word;
	}

	private void AdvanceHost()
	{
		_currentHostIndex++;
		if (_currentHostIndex >= PlayerIDs.Count) _currentHostIndex = 0;
	}

	private static string GetWord(int x)
	{
		if (x >= 100) throw new ArgumentOutOfRangeException(nameof(x), "must be less than 100!");
		if (x < 0) throw new ArgumentOutOfRangeException(nameof(x), "must be non-negative!");

		if (x == 0) return "zero";

		if (x is >= 10 and <= 19)
		{
			return x switch
			{
				10 => "ten",
				11 => "eleven",
				12 => "twelve",
				13 => "thirteen",
				14 => "fourteen",
				15 => "fifteen",
				16 => "sixteen",
				17 => "seventeen",
				18 => "eighteen",
				19 => "nineteen",
				_ => "impossible",
			};
		}
		else
		{
			string s = (x / 10) switch
			{
				2 => "twenty",
				3 => "thirty",
				4 => "fourty",
				5 => "fifty",
				6 => "sixty",
				7 => "seventy",
				8 => "eighty",
				9 => "ninety",
				0 => string.Empty,
				_ => throw new Exception($"not possible {x} / 10"),
			};

			if (x % 10 != 0)
			{
				if (x > 10) s += "-";

				s += (x % 10) switch
				{
					1 => "one",
					2 => "two",
					3 => "three",
					4 => "four",
					5 => "five",
					6 => "six",
					7 => "seven",
					8 => "eight",
					9 => "nine",
					_ => throw new Exception($"not possible {x} % 10"),
				};
			}

			return s;
		}
	}

	private void CorrectSpelling(IMessage message, string[] args)
	{
		if (_pvp && message.Author.Id == PlayerIDs[_currentHostIndex])
		{
			_guesses.Clear();
			_wordGuesses.Clear();
			_strikes = 0;

			_word = Prompt(PlayerIDs[_currentHostIndex], PromptMode.DM, (string s) => s.Length < 100, false, "What should the word be?", true).Trim().ToUpper();
			_check = _word.RemoveDiacritics();
		}
	}
}
