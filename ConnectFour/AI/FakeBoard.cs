namespace ConnectFour;
using System;

public class FakeBoard // you're welcome, thief -- Ben
{
	// this class is pretty much a copy of board, but I removed all of the security & graphics features. It also contains custom versions of my GetNextY and CheckAllDirections methods that work for FakeBoard. 

	public int Columns { get; private set; }
	public int Rows { get; private set; }
	public int Connect { get; private set; }
	public int TeamCount { get; private set; }

	private readonly State[,] _gameState;

	public State this[int x, int y]
	{
		get => _gameState[x, y];
		set => _gameState[x, y] = value;
	}

	public State this[(int x, int y) v] // tuple version
	{
		get => _gameState[v.x, v.y];
		set => _gameState[v.x, v.y] = value;
	}

	public State CurrentTeam { get; private set; }
	public State StartingTeam { get; private set; }

	public bool NoMiddleStart { get; private set; }

	public FakeBoard(Board b)
	{
		Columns = b.Columns;
		Rows = b.Rows;
		Connect = b.Connect;
		TeamCount = b.TeamCount;
		CurrentTeam = b.CurrentTeam;
		StartingTeam = b.StartingTeam;
		NoMiddleStart = b.NoMiddleStart;

		_gameState = new State[Columns, Rows];

		for (int x = 0; x < Columns; x++)
		{
			for (int y = 0; y < Rows; y++)
			{
				_gameState[x, y] = b[x, y];
			}
		}
	}

	public FakeBoard(FakeBoard b)
	{
		Columns = b.Columns;
		Rows = b.Rows;
		Connect = b.Connect;
		TeamCount = b.TeamCount;
		CurrentTeam = b.CurrentTeam;
		StartingTeam = b.StartingTeam;
		NoMiddleStart = b.NoMiddleStart;

		_gameState = new State[Columns, Rows];

		for (int x = 0; x < Columns; x++)
		{
			for (int y = 0; y < Rows; y++)
			{
				_gameState[x, y] = b[x, y];
			}
		}
	}

	#region GameLogic

	private bool TryDrop(int x, State state)
	{
		if (state == State.Empty) return false;
		if (_gameState[x, 0] != State.Empty) return false;

		int y = Rows - 1;

		while (_gameState[x, y] != State.Empty)
		{
			y--;
		}

		_gameState[x, y] = state;

		return true;
	}

	public bool CheckFull()
	{
		for (int x = 0; x < Columns; x++)
			for (int y = 0; y < Rows; y++)
				if (_gameState[x, y] == State.Empty) return false;

		return true;
	}

	public bool CheckVictor(int x, int y, State state) => state != State.Empty &&
	(
		CheckDirection(x, y, 1, 0, state) >= Connect ||
		CheckDirection(x, y, 0, 1, state) >= Connect ||
		CheckDirection(x, y, 1, 1, state) >= Connect ||
		CheckDirection(x, y, 1, -1, state) >= Connect
	);

	public bool CheckVictor((int x, int y) v, State state)
	=> CheckVictor(v.x, v.y, state);

	public int CheckDirection(int x, int y, int a, int b, State state)
	{
		if (state == State.Empty) return 0;

		int count = 1;

		int v, w;

		for (int i = 1; i < Connect; i++)
		{
			v = x + a * i;
			w = y + b * i;

			if (v >= 0 && v < Columns && w >= 0 && w < Rows && _gameState[v, w] == state) count++;
			else break;
		}

		for (int i = 1; i < Connect; i++)
		{
			v = x + -a * i;
			w = y + -b * i;

			if (v >= 0 && v < Columns && w >= 0 && w < Rows && _gameState[v, w] == state) count++;
			else break;
		}

		return count;
	}

	public int CheckDirection((int x, int y) v, int a, int b, State state) => CheckDirection(v.x, v.y, a, b, state);

	public bool InputMove(int x, int round)
	{
		if (x < 0 || x >= Columns)
		{
			Console.WriteLine($"Column {x} does not exist.");
			return false;
		}

		if (NoMiddleStart && round == 1 && CurrentTeam == StartingTeam && x == Columns / 2)
		{
			Console.WriteLine("First player cannot start in center!");
			return false;
		}

		if (TryDrop(x, CurrentTeam))
		{
			CurrentTeam = ConnectFour.Next(CurrentTeam, TeamCount);
			return true;
		}
		else
		{
			Console.WriteLine($"Column {x} is full.");
			return false;
		}
	}

	public int GetNextY(int x) // returns the y value of the next token placed in the specified column. Added by Ben.
	{
		for (int y = 0; y < Rows; y++)
		{
			if (this[x, y] != State.Empty)
			{
				return y - 1;
			}
		}

		return Rows - 1;
	}

	public int CheckAllDirections(int x, int y, State state) =>
		CheckDirection(x, y, 1, 0, state) +
		CheckDirection(x, y, 0, 1, state) +
		CheckDirection(x, y, 1, 1, state) +
		CheckDirection(x, y, 1, -1, state); // counts how many tokens a token placed here would connect to. Added by Ben.

	#endregion

	public void Set(Board b)
	{
		Columns = b.Columns;
		Rows = b.Rows;
		Connect = b.Connect;
		TeamCount = b.TeamCount;
		CurrentTeam = b.CurrentTeam;
		StartingTeam = b.StartingTeam;
		NoMiddleStart = b.NoMiddleStart;

		//_gameState = new State[Columns, Rows];

		for (int x = 0; x < Columns; x++)
		{
			for (int y = 0; y < Rows; y++)
			{
				_gameState[x, y] = b[x, y];
			}
		}
	}
}
