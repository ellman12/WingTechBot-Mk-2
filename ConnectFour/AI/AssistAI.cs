using System;
namespace ConnectFour
{
    public class AssistAI : ProAI, IHuman
    {
        public override string Name => "AssistAI";

        public ulong ID { get; private set; }
        private bool init = false;

        private ConnectFour game;

        public override void Init(Func<object, Discord.IMessage> saveWriteLine)
        {
            base.Init(saveWriteLine);
            Say("I'll help you out here.");
        }


        public void Init(ulong id, ConnectFour game)
        {
            if (!init)
            {
                init = true;
                ID = id;
                this.game = game;
            }
        }

        public override int Prompt(Board board, int round)
        {
            Say($"Input move, {board.CurrentTeam} team.");

            int numberInput = -1;
            int lastInput = -2;

            bool confirmed = false;
            while (!confirmed)
            {
                int? move = game.PromptMove(ID);

                if (move == null) board.Forfeit();

                numberInput = move ?? 0;

                int[] values = GetValues(new FakeBoard(board));

                int max = Library.Max(values);

                if ((values[max] > 500 && max != numberInput) ||
                    (values[numberInput] < 0 && values[max] > 0))
                {
                    if (lastInput == numberInput) confirmed = true;
                    else
                    {
                        Say($"Are you sure you want that? {max} may be a better option. Think carefully.");

                        lastInput = numberInput;
                    }
                }
                else confirmed = true;
            }

            return numberInput;
        }

        public override void MatchEnd(State victor, int round) // This is called every time a round ends.
        {
            if (victor == State.Empty) Say("Good stuff, human.");
            else if (victor == Team) Say("Good stuff, human.");
            else Say("We'll get them next time.");

        }

        public override void GameEnd() // This is called at the end of a series of games.
        {
            Say("I hope I was helpful.");
        }
    }
}