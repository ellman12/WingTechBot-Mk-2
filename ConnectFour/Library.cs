using System;
using System.Collections.Generic;

namespace ConnectFour
{
    public static class Library // feel free to add your own methods here!
    {
        public static Random Random { get; private set; } = new(); // a random all set up and ready to use... Try Random.Next();

        public static bool TryDec(string hex, out int dec) // this takes a string in hex form and changes it into decimal form. Added by Ben.
        {
            return int.TryParse(hex,
            System.Globalization.NumberStyles.HexNumber,
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture,
             out dec);
        }

        public static int GetNextY(int x, Board board) // returns the y value of the next token placed in the specified column. Added by Ben.
        {
            for (int y = 0; y < board.Rows; y++)
            {
                if (board[x, y] != State.Empty)
                {
                    return y - 1;
                }
            }

            return board.Rows - 1;
        }

        public static int CheckAllDirections(int x, int y, State state, Board board) => // counts how many tokens a token placed here would connect to. Added by Ben.
            board.CheckDirection(x, y, 1, 0, state) +
            board.CheckDirection(x, y, 0, 1, state) +
            board.CheckDirection(x, y, 1, 1, state) +
            board.CheckDirection(x, y, 1, -1, state);

        public static int Max(int[] array) // returns the index of the largest value in the given array. Added by Ben.
        {
            int maxIndex = 0;
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] > array[maxIndex]) maxIndex = i;
            }

            List<int> indexes = new();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == array[maxIndex]) indexes.Add(i);
            }

            return indexes[Random.Next(indexes.Count)];
        }

        public static int Max(double[] array) // returns the index of the largest value in the given array. Added by Ben.
        {
            int maxIndex = 0;
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] > array[maxIndex]) maxIndex = i;
            }

            List<int> indexes = new();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == array[maxIndex]) indexes.Add(i);
            }

            return indexes[Random.Next(indexes.Count)];
        }

        /*
        Other methods, properties, etc. you can use!

        // MainClass

        static int IntQuery(string text) // Writes text and Reads Console until user inputs an integer, then returns integer.

        static bool BoolQuery(string text) // Writes text and Reads Console until user inputs y or n, then returns input == "y".

        static State Next(State state, int teamCount) // says which team goes after the specified team. Use Board.TeamCount to get how many teams there are.

        // AI

        int Auth // returns your AI's auth token, which you can use as an added layer of security. Every time the game prompts you, it will send your auth token to you. Only the game and you know your auth token. Can only be set once (The game handles that for you).

        State Team // returns the Team your AI is on. Can only be set once (The game handles that for you).

        void Say(string input) // recommended for dialogue, it is equivalent to Console.WriteLine($"{Name}: {input}");

        // Board

        int Columns // returns how many columns there are on the current board.

        int Rows // returns how many rows there are on the current board.

        int Connect // returns how many dots the player must connect to win.

        int TeamCount // returns how many teams there are.

        State [] // An indexer wrapping a 2D array containing the states of each slot on the board. [0, 0] is the top-left corner, [Board.Columns - 1, Board.Rows - 1] is the bottom-right. Use this to see what is on the board. If you want to get a value, use Board[x, y].

        State CurrentTeam // returns the team who should respond next.

        State StartingTeam // returns which team moves first.

        static bool GameInProgress // returns true if a Board is created and active.

        State Victor // returns who won the game, or State.Empty in the case of a draw or ongoing game.

        bool NoMiddleStart // returns whether the first player is allowed to start in the middle or not.

        bool CheckFull() // returns whether or not the board is full.

        bool CheckVictor(int x, int y, State state) // checks whether or not the specified team would win if they were to place a token in the specified space.

        int CheckDirection(int x, int y, int a, int b, State state) // this one needs a bit more explanation. This method pretends that the specified team placed a token at [x, y]. It then checks in the direction (a, b) and (-a, -b) to see how many tokens of team state would be lined up in that direction. Example directions: (1, 0) is horizontal, (0, 1) is vertical, (1, 1) is downwards diagonal, (1, -1) is upwards diagonal. Returns how many tokens have been lined up. Hint: you can compare the result of this method to Board.Connect to see how good a possible move is.

        void Forfeit() // forfeits the game.

        void Draw(int round) // Draws the game board, with the specified round.

        static string GetWord(int x) // for integers 0-16, returns a string with that number written out in words.

        // Game
        // theoretically these are available... good luck getting a reference to a Game instance though.

        int Round // returns what round the game is on.

        static bool GameInProgress // returns whether or not a game is in progress.

        State PromptingTeam // returns which team the Game is waiting on to respond.

        Board Board // returns the current board.

        */
    }
}