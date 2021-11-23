namespace ConnectFour;
using System;

public class ProAI : AI
{
    public override string Name => "ProAI";

    public override void Init(Func<object, Discord.IMessage> saveWriteLine)
    {
        base.Init(saveWriteLine);
        if (this is not AssistAI) Say("Good luck, have fun.");
    }

    public override int Prompt(Board board, int round) // Here is where he thinks.
    {
        int[] values = GetValues(new FakeBoard(board));

        int column = Library.Max(values);

        if (column < 0 || column >= board.Columns) column = new Random().Next(board.Columns);

        // log thinking
        /*for (int x = 0; x < board.Columns; x++) Console.WriteLine($"{x}: {values[x]}");
        Console.WriteLine($"Next move: {column}");
        Console.ReadLine();*/

        return column;
    }

    protected int[] GetValues(FakeBoard board)
    {
        int[] values = new int[board.Columns];

        for (int x = 0; x < board.Columns; x++)
        {
            int y = board.GetNextY(x);

            if (y == -1)
            {
                values[x] = -9999;
                continue;
            }

            values[x] += SmartCheck(x, y, Team, board) * (board.TeamCount - 1); // get those points

            if (y != 0)
            {
                values[x] -= SmartCheck(x, y - 1, Team, board) * (board.TeamCount - 1);
                if (board.CheckVictor(x, y - 1, Team)) values[x] -= 100;
            }

            if (board.CheckVictor(x, y, Team)) values[x] += 99999; // take that dub
            double teamValueMultiplier = 1.0;
            State teamCheck = Team;
            for (int i = 0; i < board.TeamCount - 1; i++)
            {
                teamCheck = ConnectFour.Next(teamCheck, board.TeamCount);
                int value = 0, otherValue = 0;
                value += SmartCheck(x, y, teamCheck, board); // block those fools

                if (y > 0 && board.CheckVictor(x, y - 1, teamCheck)) // don't give free victories unless checkmated.
                {
                    value += -500;
                }

                if (board.CheckVictor(x, y, teamCheck)) // block possible losses
                {
                    value += 2000;
                }

                if (x < board.Columns - (board.Connect - 1)) // no 2 lines
                {
                    int otherX = x + (board.Connect - 1);
                    int otherY = board.GetNextY(otherX);

                    if (y == otherY)
                    {
                        bool found = false;
                        for (int j = x + 1; j < otherX; j++)
                        {
                            if (board[j, y] != teamCheck)
                            {
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            value += 1000;
                            otherValue += 1000;
                            otherValue = (int)(otherValue * teamValueMultiplier);
                            values[otherX] += otherValue;
                        }
                    }
                }

                value = (int)(value * teamValueMultiplier);
                values[x] += value;

                teamValueMultiplier *= 0.9;
            }
        }

        return values;
    }

    private int SmartCheck(int x, int y, State state, FakeBoard board)
    {
        int vertical = 0, horizontal = 0, upDiagonal = 0, downDiagonal = 0;

        if (y >= board.Connect || board[x, board.Connect - 1] == Team) vertical = board.CheckDirection(x, y, 0, 1, state);

        if (x == 0)
        {
            if (board[x + board.Connect - 1, y] == state) horizontal = board.CheckDirection(x, y, 1, 0, state) + 2;

            if (y <= board.Rows - board.Connect && board[x + board.Connect - 1, y + board.Connect - 1] == state) upDiagonal = board.CheckDirection(x, y, 1, -1, state) + 1;
            if (y >= board.Connect - 1 && board[x + board.Connect - 1, y - board.Connect + 1] == state) downDiagonal = board.CheckDirection(x, y, 1, 1, state) + 1;
        }
        else if (x == board.Columns - 1)
        {
            if (board[x - board.Connect + 1, y] == state) horizontal = board.CheckDirection(x, y, 1, 0, state);

            if (y <= board.Rows - board.Connect && board[x - board.Connect + 1, y + board.Connect - 1] == state) downDiagonal = board.CheckDirection(x, y, 1, 1, state);
            if (y >= board.Connect - 1 && board[x - board.Connect + 1, y - board.Connect + 1] == state) upDiagonal = board.CheckDirection(x, y, 1, -1, state);
        }
        else return board.CheckAllDirections(x, y, state);

        int value = vertical;
        if (horizontal > value) value = horizontal;
        if (upDiagonal > value) value = upDiagonal;
        if (downDiagonal > value) value = downDiagonal;

        return value;
    }

    public override void MatchEnd(State victor, int round) // This is called every time a round ends.
    {
        if (victor == State.Empty) Say("A draw? That never happens!");
        else if (victor == Team) Say("Good game, well played.");
        else Say("Good game, well played.");
    }

    public override void GameEnd() => Say("Thanks for playing!"); // This is called at the end of a series of games.
}
