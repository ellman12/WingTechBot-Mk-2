namespace ConnectFour;
using System;

public abstract class AI
{
    // you can make your AI say a greeting message in its constructor if you want.
    // remember that this game is customizable! You might be playing connect 8 on a 16x16 board for all you know.
    // MainClass.Next, 

    // State[x, y] --- the game state array. For x: 0 = far left column, 6 (normally) = far right column. For y: 0 = top row. 5 (normally) = bottom row.
    // State can be Empty, Red, Yellow, Green, or Blue.
    // round --- shows which round the game is on.
    // victor --- which player won the game? Empty = draw, Cross = player1, Circle = player2.
    // the turn order rotates every match.

    public abstract string Name { get; } // Your AI's name.

    public abstract int Prompt(Board board, int round); //This is where your AI thinks. Return an integer from 0 - (normally) 6 representing the chosen move. Check out the Board class to see what methods/properties are accessible for your calculations.

    public virtual void MatchEnd(State victor, int round) { } // use this to make your AI respond to its win/loss.

    public virtual void GameEnd() { } // use this to make your AI respond to the game ending.

    public virtual void Init(Func<object, Discord.IMessage> saveWriteLine) => _saveWriteLine = saveWriteLine; // what does your AI say when created?

    private bool _setTeam = false;
    private State _team;
    public State Team // stores which team your AI is on. Can only be set once.
    {
        get => _team;
        set
        {
            if (!_setTeam)
            {
                _setTeam = true;
                _team = value;
            }
        }
    }

    private Func<object, Discord.IMessage> _saveWriteLine;

    public void Say(string text) => _saveWriteLine($"{Name}: {text}"); // say some dialogue!
}
