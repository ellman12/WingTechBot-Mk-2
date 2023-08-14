namespace ConnectFour;
using System;

public class Board
{
	public int Columns { get; private set; }
	public int Rows { get; private set; }
	public int Connect { get; private set; }
	public int TeamCount { get; private set; }

	private readonly State[,] _gameState;

	public State this[int x, int y]
	{
		get => _gameState[x, y];
		private set => _gameState[x, y] = value;
	}

	public State this[(int x, int y) v] // tuple version
	{
		get => _gameState[v.x, v.y];
		private set => _gameState[v.x, v.y] = value;
	}

	public State CurrentTeam { get; private set; }
	public State StartingTeam { get; private set; }
	public State Victor { get; private set; } = State.Empty;

	public static bool GameInProgress { get; private set; }
	public bool NoMiddleStart { get; private set; }
	public bool EnableCreationAllowed { get; private set; } = false;
	public bool DisableCreationAllowed { get; private set; } = false;

	public string MoveHistory { get; private set; } = "";

	private int _round;
	private readonly int _auth;
	private bool _authSent = false;
	public int Auth
	{
		get
		{
			if (!_authSent)
			{
				_authSent = true;
				return _auth;
			}
			else
			{
				return -1;
			}
		}
	}

	private readonly Func<object, Discord.IMessage> _saveWriteLine;
	private readonly Action _clear;

	public Board(Func<object, Discord.IMessage> saveWriteLine, Action clear, int columns = 7, int rows = 6, int connect = 4, int teams = 2, bool noMiddleStart = false, State currentTeam = State.Circle)
	{
		//if (GameInProgress) throw new("A board is already in use."); // $$$ maybe reintroduce?

		if (connect > columns && connect > rows)
		{
			throw new("Connect cannot be larger than columns and rows.");
		}

		if (teams >= Enum.GetValues(typeof(State)).Length)
		{
			throw new("There can be no more than four teams.");
		}

		GameInProgress = true;

		Columns = columns;
		Rows = rows;
		Connect = connect;
		TeamCount = teams;
		CurrentTeam = currentTeam;
		StartingTeam = currentTeam;
		NoMiddleStart = noMiddleStart;

		_saveWriteLine = saveWriteLine;
		_clear = clear;

		_gameState = new State[columns, rows];
		_auth = new Random().Next(int.MaxValue);
	}

	public void EnableCreation(int auth)
	{
		if (auth == _auth)
		{
			EnableCreationAllowed = false;
			GameInProgress = false;
		}
	}

	public void DisableCreation(int auth)
	{
		if (auth == _auth)
		{
			DisableCreationAllowed = false;
			GameInProgress = true;
		}
	}

	#region GameLogic

	private bool TryDrop(int x, State state)
	{
		if (state == State.Empty)
		{
			return false;
		}

		if (_gameState[x, 0] != State.Empty)
		{
			return false;
		}

		var y = Rows - 1;

		while (_gameState[x, y] != State.Empty)
		{
			y--;
		}

		_gameState[x, y] = state;

		MoveHistory += $"{x:X}";

		if (CheckFull())
		{
			EndGame();
		}

		if (CheckVictor(x, y, _gameState[x, y]))
		{
			Victor = _gameState[x, y];
			EndGame();
		}

		return true;
	}

	public bool CheckFull()
	{
		for (var x = 0; x < Columns; x++)
		{
			for (var y = 0; y < Rows; y++)
			{
				if (_gameState[x, y] == State.Empty)
				{
					return false;
				}
			}
		}

		return true;
	}

	public bool CheckVictor(int x, int y, State state) => state != State.Empty &&
	(
		CheckDirection(x, y, 1, 0, state) >= Connect ||
		CheckDirection(x, y, 0, 1, state) >= Connect ||
		CheckDirection(x, y, 1, 1, state) >= Connect ||
		CheckDirection(x, y, 1, -1, state) >= Connect
	);

	public bool CheckVictor((int x, int y) v, State state) => CheckVictor(v.x, v.y, state); // tuple version

	public int CheckDirection(int x, int y, int a, int b, State state)
	{
		if (state == State.Empty)
		{
			return 0;
		}

		var count = 1;

		int v, w;

		for (var i = 1; i < Connect; i++)
		{
			v = x + a * i;
			w = y + b * i;

			if (v >= 0 && v < Columns && w >= 0 && w < Rows && _gameState[v, w] == state)
			{
				count++;
			}
			else
			{
				break;
			}
		}

		for (var i = 1; i < Connect; i++)
		{
			v = x + -a * i;
			w = y + -b * i;

			if (v >= 0 && v < Columns && w >= 0 && w < Rows && _gameState[v, w] == state)
			{
				count++;
			}
			else
			{
				break;
			}
		}

		return count;
	}

	public int CheckDirection((int x, int y) v, (int x, int y) d, State state) => CheckDirection(v.x, v.y, d.x, d.y, state);

	public bool InputMove(int x, int auth, int round)
	{
		if (auth != _auth)
		{
			_saveWriteLine("Wrong auth token.");
			return false;
		}

		if (x < 0 || x >= Columns)
		{
			_saveWriteLine($"Column {x} does not exist.");
			return false;
		}

		if (NoMiddleStart && round == 1 && CurrentTeam == StartingTeam && x == Columns / 2)
		{
			_saveWriteLine("First player cannot start in center!");
			return false;
		}

		_round = round;

		if (TryDrop(x, CurrentTeam))
		{
			CurrentTeam = ConnectFour.Next(CurrentTeam, TeamCount);
			return true;
		}
		else
		{
			_saveWriteLine($"Column {x} is full.");
			return false;
		}
	}

	private void EndGame()
	{
		Draw(_round);

		_saveWriteLine("Game Over.");
		if (Victor == State.Empty)
		{
			_saveWriteLine("Draw.");
		}
		else
		{
			_saveWriteLine($"{Victor} Team wins!");
		}

		EnableCreationAllowed = true; // gives Game control over this.GameInProgress
		DisableCreationAllowed = true; // same
		GameInProgress = false;
	}

	public void Forfeit()
	{
		Victor =
			TeamCount > 2
			? State.Empty
			: ConnectFour.Next(CurrentTeam, TeamCount);

		EndGame();
	}

	#endregion

	#region Graphics

	public void Draw(int round)
	{
		_clear();

		var screen = $"```\nConnect {GetWord(Connect)}\n" +
			$"Round: {round}\n";

		WriteHistory(ref screen);

		DrawTop(ref screen);

		for (var y = 0; y < Rows; y++)
		{
			screen += "|";

			for (var x = 0; x < Columns; x++)
			{
				DrawSlot(_gameState[x, y], ref screen);
			}

			screen += "\n";
		}

		DrawBottom(ref screen);

		screen += "```";
		_saveWriteLine(screen);
	}

	public static string GetWord(int x) => x switch
	{
		0 => "Zero",
		1 => "One",
		2 => "Two",
		3 => "Three",
		4 => "Four",
		5 => "Five",
		6 => "Six",
		7 => "Seven",
		8 => "Eight",
		9 => "Nine",
		10 => "Ten",
		11 => "Eleven",
		12 => "Twelve",
		13 => "Thirteen",
		14 => "Fourteen",
		15 => "Fifteen",
		16 => "Sixteen",
		_ => x.ToString(),
	};

	private void WriteHistory(ref string screen)
	{
		screen += "History: ";

		var team = StartingTeam;

		for (var i = 0; i < MoveHistory.Length; i++)
		{
			screen += MoveHistory[i];
			team = ConnectFour.Next(team, TeamCount);
		}

		screen += '\n';
	}

	private static void DrawSlot(State state, ref string screen)
	{
		if (state == State.Empty)
		{
			screen += "   ";
		}
		else
		{
			screen += $" {GetGlyph(state)} ";
		}

		screen += "|";
	}

	public static ConsoleColor GetColor(State state) => state switch
	{
		State.Cross => ConsoleColor.Yellow,
		State.Circle => ConsoleColor.Red,
		State.Ampersand => ConsoleColor.Green,
		State.At => ConsoleColor.Blue,
		State.Pound => ConsoleColor.Magenta,
		State.Plus => ConsoleColor.DarkYellow,
		State.Dollar => ConsoleColor.White,
		State.Question => ConsoleColor.DarkGray,
		_ => ConsoleColor.White,
	};

	public static char GetGlyph(State state) => state switch
	{
		State.Circle => 'O',
		State.Cross => 'X',
		State.Ampersand => '&',
		State.At => '@',
		State.Pound => '#',
		State.Plus => '+',
		State.Dollar => '$',
		State.Question => '?',
		_ => 'Q',
	};

	private void DrawTop(ref string screen)
	{
		var width = Columns * 4 + 1;

		for (var i = 0; i < width; i++)
		{
			screen += '=';
		}

		screen += '\n';
	}

	private void DrawBottom(ref string screen)
	{
		screen += '=';

		for (var i = 0; i < Columns; i++)
		{
			screen += $"[{i:X}]=";
		}

		screen += '\n';
	}

	#endregion
}
