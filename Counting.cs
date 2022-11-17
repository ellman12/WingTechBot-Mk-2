namespace WingTechBot;
using System.Diagnostics;

public class Counting : Game
{
	private int _countBy;
	private bool _turnOrder;
	private int _currentPlayerIndex = 0;

	protected override bool Debug => false;

	protected override void Start()
	{
		_countBy = Prompt<int>(GamemasterID, AllowedChannels, true, "What are we gonna count by?");
		_turnOrder = Prompt<bool>(GamemasterID, AllowedChannels, true, "Is turn order required? (true/false)");
	}

	public override void RunGame()
	{
		if (PlayerIDs.Count == 0)
		{
			WriteLine("You can't count with zero players!");
			return;
		}

		Stopwatch timer = new();
		var score = 0;

		WriteLine($"Alright, start counting by {_countBy}'s!");
		timer.Start();

		while (true)
		{
			(var id, var guess) = PromptAny<int>(PromptMode.Any, true);

			if ((!_turnOrder || PlayerIDs[_currentPlayerIndex] == id) && guess == ++score * _countBy)
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
		_currentPlayerIndex++;
		if (_currentPlayerIndex >= PlayerIDs.Count)
		{
			_currentPlayerIndex = 0;
		}
	}
}
