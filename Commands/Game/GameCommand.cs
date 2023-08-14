namespace WingTechBot;
using System;
using System.Linq;

internal class GameCommand : Command
{
	private Game _createGame;

	public override void Execute()
	{
		if (arguments.Length <= 1)
		{
			throw new ArgumentException("Please type the name of the game you would like to play after the command. For example: \"~game connectfour\"");
		}

		if (Program.GameHandler.ActiveGames.Any(g => g.PlayerIDs.Contains(message.Author.Id)))
		{
			throw new($"You are already part of an active game!");
		}

		var foundGame = Program.GameHandler.Games.FirstOrDefault((Type t) => t.Name.ToLower() == arguments[1].ToLower());

		if (foundGame is null)
		{
			throw new ArgumentException($"The game \"{arguments[1]}\" could not be found.");
		}

		_createGame = Activator.CreateInstance(foundGame) as Game;

		_createGame.Init(message);

		Program.GameHandler.ActiveGames.Add(_createGame);

		new System.Threading.Thread(_createGame.Run).Start();
	}

	public override string LogString => $"started game: {_createGame.GetType().Name}";
	public override string[] Aliases => new[] { "game", "play", "playgame", "g" };
}
