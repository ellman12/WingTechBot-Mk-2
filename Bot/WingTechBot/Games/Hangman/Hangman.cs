namespace WingTechBot.Games.Hangman;

public sealed class Hangman : Game
{
	private string word = "", check = "";
	private readonly List<char> guesses = [];
	private readonly List<string> wordGuesses = [];

	private Dictionary<IUser, int> scores;

	private int currentHostIndex = -1, strikes = 0, score = 0, total = 0;

	private bool pvp;

	private IUser CurrentHost => Players[currentHostIndex];

	private static readonly string[] heads =
	[
		"(O.O)",
		"(O.-)",
		"(-.-)",
		"(O.o)",
		"(>.<)",
		"(>.>)",
		"(X.X)",
		"(@.@)",
		"(o-o)",
		"(-_-)",
		"(UwU)",
		"(OwO)", //<-- me_irl when I'm about to die if the player doesn't guess the right word.
		"(=.=)",
		"(OxO)",
		"(o_o)",
		"(._.)",
		"(;_;)",
		"(^.^)",
		"(~.~)"
	];

	public override async Task GameSetup()
	{
		pvp = !await UserInput.PromptYN(ThreadChannel, "Would you like to face a bot? ", CancelTokenSource.Token);

		while (!Players.Any() || (pvp && Players.Count == 1))
		{
			if (!Players.Any())
			{
				await SendMessage("You can't play hangman with zero players!");
				await GetPlayers();
			}

			if (pvp && Players.Count == 1)
			{
				await SendMessage("You need at least two players to play multiplayer.");
				await GetPlayers();
			}
		}

		if (pvp)
		{
			scores = new Dictionary<IUser, int>();

			foreach (var id in Players)
			{
				scores.Add(id, 0);
			}
		}
	}

	public override async Task RunGame()
	{
		while (true)
		{
			await RunRound();
		}
	}

	private async Task RunRound()
	{
		if (pvp)
		{
			AdvanceHost();
			await SendMessage($"Prompting {CurrentHost.Username} for the word...");
			word = (await UserInput.Prompt(await CurrentHost.CreateDMChannelAsync(), "What should the word be?", CancelTokenSource.Token)).Trim().ToUpper();
		}
		else
		{
			word = Words[Random.Shared.Next(0, Words.Length)].Trim().ToUpper();
		}

		;
	}

	private void AdvanceHost()
	{
		currentHostIndex++;
		if (currentHostIndex >= Players.Count)
			currentHostIndex = 0;
	}

	protected override async Task ProcessMessage(SocketMessage message) {}
}