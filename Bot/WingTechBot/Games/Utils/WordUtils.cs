namespace WingTechBot.Games.Utils;

public static class WordUtils
{
	///List of words that can be used for games like <see cref="Hangman"/>
	public static string[] Words { get; } = File.ReadAllLines(Path.Combine(Program.ProjectRoot, "Games", "words.txt"));

	///List of words that probably shouldn't be used in games like <see cref="Hangman"/>
	public static string[] BannedWords { get; } = File.ReadAllLines(Path.Combine(Program.ProjectRoot, "Games", "banned.txt"));

	public static string GetRandomWord(
		bool allowCurses = false,
		bool shouldAmericanize = false,
		int minLength = 0,
		int maxLength = int.MaxValue,
		Predicate<string> filterFn = null
	)
	{
		while (true)
		{
			var word = Words[Random.Shared.Next(0, Words.Length)];

			if (!allowCurses && BannedWords.Contains(word))
			{
				continue;
			}

			if (shouldAmericanize && !word.IsAmericanized())
			{
				continue;
			}

			if (word.Length < minLength || word.Length > maxLength)
			{
				continue;
			}

			if (filterFn != null && !filterFn(word))
			{
				continue;
			}

			return word;
		}
	}

	public static string GetNumberName(int numLessThan100)
	{
		var x = numLessThan100;
		if (x >= 100)
		{
			throw new ArgumentOutOfRangeException(nameof(numLessThan100), "must be less than 100!");
		}

		if (x < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(numLessThan100), "must be non-negative!");
		}

		if (x == 0)
		{
			return "zero";
		}

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
			var s = (x / 10) switch
			{
				2 => "twenty",
				3 => "thirty",
				4 => "fourty",
				5 => "fifty",
				6 => "sixty",
				7 => "seventy",
				8 => "eighty",
				9 => "ninety",
				0 => "",
				_ => throw new($"not possible {x} / 10"),
			};

			if (x % 10 != 0)
			{
				if (x > 10)
				{
					s += "-";
				}

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
					_ => throw new($"not possible {x} % 10"),
				};
			}

			return s;
		}
	}
}