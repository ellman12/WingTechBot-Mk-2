namespace ConnectFour
{
    public class Human : AI, IHuman
    {
        public override string Name => "Human";

        public ulong ID { get; private set; }
        private bool init = false;

        private ConnectFour game;

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

            int? move = game.PromptMove(ID);

            if (move == null) board.Forfeit();

            return move ?? 0;
        }
    }
}