namespace WingTechBot.Games.Hangman;

public sealed class Hangman : Game
{
	private string word = "", check = "";
	private readonly List<char> guesses = [];
	private readonly List<string> wordGuesses = [];

	private Dictionary<IUser, int> scores;

	private int currentHostIndex = -1, strikes = 0, score = 0, total = 0;

	private bool pvp;

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
		throw new NotImplementedException();
	}

	private async Task RunRound() {}

	protected override async Task ProcessMessage(SocketMessage message) {}
}