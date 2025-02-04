namespace WingTechBot.Games.Utils;

internal static class DiacriticUtils
{
	// adapted from CodeIgniter and CIRCLE:
	// https://stackoverflow.com/a/34272324

	private static readonly Dictionary<string, string> _foreign_characters = new()
	{
		{ "ÀÁÂÃÄÅǺĀĂĄǍΑΆẢẠẦẪẨẬẰẮẴẲẶÄА", "A" },
		{ "àáâãåǻāăąǎªαάảạầấẫẩậằắẵẳặаä", "a" },
		{ "Б", "B" },
		{ "б", "b" },
		{ "ÇĆĈĊČ", "C" },
		{ "çćĉċč", "c" },
		{ "Д", "D" },
		{ "д", "d" },
		{ "ÐĎĐΔ", "Dj" },
		{ "ðďđδ", "dj" },
		{ "ÈÉÊËĒĔĖĘĚΕΈẼẺẸỀẾỄỂỆЕЭ", "E" },
		{ "èéêëēĕėęěέεẽẻẹềếễểệеэ", "e" },
		{ "Ф", "F" },
		{ "ф", "f" },
		{ "ĜĞĠĢΓГҐ", "G" },
		{ "ĝğġģγгґ", "g" },
		{ "ĤĦ", "H" },
		{ "ĥħ", "h" },
		{ "ÌÍÎÏĨĪĬǏĮİΗΉΊΙΪỈỊИЫ", "I" },
		{ "ìíîïĩīĭǐįıηήίιϊỉịиыї", "i" },
		{ "Ĵ", "J" },
		{ "ĵ", "j" },
		{ "ĶΚК", "K" },
		{ "ķκк", "k" },
		{ "ĹĻĽĿŁΛЛ", "L" },
		{ "ĺļľŀłλл", "l" },
		{ "М", "M" },
		{ "м", "m" },
		{ "ÑŃŅŇΝН", "N" },
		{ "ñńņňŉνн", "n" },
		{ "ÒÓÔÕŌŎǑŐƠØǾΟΌΩΏỎỌỒỐỖỔỘỜỚỠỞỢОÖ", "O" },
		{ "òóôõōŏǒőơøǿºοόωώỏọồốỗổộờớỡởợоö", "o" },
		{ "П", "P" },
		{ "п", "p" },
		{ "ŔŖŘΡР", "R" },
		{ "ŕŗřρр", "r" },
		{ "ŚŜŞȘŠΣС", "S" },
		{ "śŝşșšſσςс", "s" },
		{ "ȚŢŤŦτТ", "T" },
		{ "țţťŧт", "t" },
		{ "ÙÚÛŨŪŬŮŰŲƯǓǕǗǙǛŨỦỤỪỨỮỬỰÜУ", "U" },
		{ "ùúûũūŭůűųưǔǖǘǚǜυύϋủụừứữửựüу", "u" },
		{ "ÝŸŶΥΎΫỲỸỶỴЙ", "Y" },
		{ "ýÿŷỳỹỷỵй", "y" },
		{ "В", "V" },
		{ "в", "v" },
		{ "Ŵ", "W" },
		{ "ŵ", "w" },
		{ "ŹŻŽΖЗ", "Z" },
		{ "źżžζз", "z" },
		{ "ƒ", "f" },
		{ "π", "p" },
		{ "β", "v" },
		{ "μ", "m" },
	};

	public static char RemoveDiacritics(this char c)
	{
		foreach (var entry in _foreign_characters)
		{
			if (entry.Key.IndexOf(c) != -1)
			{
				return entry.Value[0];
			}
		}

		return c;
	}

	public static string RemoveDiacritics(this string s)
	{
		var text = "";

		foreach (var c in s)
		{
			var len = text.Length;

			foreach (var entry in _foreign_characters)
			{
				if (entry.Key.IndexOf(c) != -1)
				{
					text += entry.Value;
					break;
				}
			}

			if (len == text.Length)
			{
				text += c;
			}
		}

		return text;
	}

	public static bool IsAmericanized(this char c) => AmericanLetters.Contains(c);

	public static string AmericanLetters => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

	public static bool IsAmericanized(this string s)
	{
		var output = true;
		foreach (var c in s)
		{
			if (char.IsLetter(c))
			{
				output &= IsAmericanized(c);
			}
		}

		return output;
	}
}