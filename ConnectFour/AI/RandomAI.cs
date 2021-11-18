using System;
namespace ConnectFour
{
    public class RandomAI : AI
    {
        readonly Random random = new(); // only needed for random moves

        public override string Name => "RandomAI"; // make sure you change this

        public override void Init(Func<object, Discord.IMessage> saveWriteLine) // This code runs when RandomAI is selected.
        {
            base.Init(saveWriteLine); // must include this line!!!
            Say("I'm so random heehee XD"); // use Say(string text) to say dialogue!
        }

        public override int Prompt(Board board, int round) // Here is where he thinks. Return an int corresponding to the column you want to drop your next token in. For columns A-F, return numbers 10-15. You can call ConnectLibrary.Dec(string s) to convert a number/letter from hex into decimal 0-15.
        {
            return random.Next(board.Columns); // Aaaand he just puts in a random move. Make sure your AI knows how many columns there are, it's not always 7!
        }

        public override void MatchEnd(State victor, int round) // This is called every time a round ends.
        {
            if (victor == State.Empty) Say("A draw? You can't ever defeat me! >;)"); // draw dialogue
            else if (victor == Team) Say("Looks like I won, heehee! :)"); // win dialogue
            else Say("I lost... But I'll get you next time! :`("); // loss dialogue
        }

        public override void GameEnd() // This is called at the end of a series of games.
        {
            Say("It was fun playing with you, teehee!");
        }
    }
}