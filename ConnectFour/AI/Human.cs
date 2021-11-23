namespace ConnectFour
{
    public class Human : AI, IHuman
    {
        public override string Name => "Human";

        public ulong ID { get; private set; }
        private bool _init = false;

        private ConnectFour _game;

        public void Init(ulong id, ConnectFour game)
        {
            if (!_init)
            {
                _init = true;
                ID = id;
                this._game = game;
            }
        }

        public override int Prompt(Board board, int round)
        {
            Say($"Input move, {board.CurrentTeam} team.");

            int? move = _game.PromptMove(ID);

            if (move is null) board.Forfeit();

            return move ?? 0;
        }
    }
}