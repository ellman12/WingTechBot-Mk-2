using System.Diagnostics;

namespace WingTechBot
{
    public class Counting : Game
    {
        private int countBy;
        private bool turnOrder;

        int currentPlayerIndex = 0;

        protected override bool Debug => false;

        protected override void Start()
        {
            countBy = Prompt<int>(GamemasterID, AllowedChannels, true, "What are we gonna count by?");
            turnOrder = Prompt<bool>(GamemasterID, AllowedChannels, true, "Is turn order required? (true/false)");
        }

        public override void RunGame()
        {
            if (PlayerIDs.Count == 0)
            {
                WriteLine("You can't count with zero players!");
                return;
            }

            Stopwatch timer = new();
            int score = 0;

            WriteLine($"Alright, start counting by {countBy}'s!");
            timer.Start();

            while (true)
            {
                (ulong id, int guess) response = PromptAny<int>(PromptMode.Any, true);

                if ((!turnOrder || PlayerIDs[currentPlayerIndex] == response.id) && response.guess == ++score * countBy)
                {
                    Advance();
                }
                else
                {
                    timer.Stop();
                    break;
                }
            }

            WriteLine($"Gameover! Score: {score - 1}; Time: {timer.Elapsed} seconds");
        }

        private void Advance()
        {
            currentPlayerIndex++;
            if (currentPlayerIndex >= PlayerIDs.Count) currentPlayerIndex = 0;
        }
    }
}
