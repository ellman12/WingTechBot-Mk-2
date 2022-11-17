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

internal class ListGamesCommand : Command
{
	public override void Execute()
	{
		if (Program.GameHandler.Games.Length > 0)
		{
			var list = "Available Games:\n";
			foreach (var game in Program.GameHandler.Games)
			{
				list += $" - {game.Name}\n";
			}

			message.Channel.SendMessageAsync(list);
		}
		else
		{
			message.Channel.SendMessageAsync("There are no available games.");
		}
	}

	public override string LogString => "listed available games";
	public override string[] Aliases => new[] { "listgames", "listgame", "lg", "list", "games" };
}

internal class ActiveGamesCommand : Command
{
	public override void Execute()
	{
		if (Program.GameHandler.ActiveGames.Count > 0)
		{
			var list = "Current Games:\n";
			foreach (var game in Program.GameHandler.ActiveGames)
			{
				var gm = Program.GetUser(game.GamemasterID);
				list += $"{game.GetType().Name} (GM: {gm.Username}#{gm.Discriminator}):\n";
				foreach (var id in game.PlayerIDs)
				{
					var user = Program.GetUser(id);
					list += $" - {user.Username}#{user.Discriminator}\n";
				}
			}

			message.Channel.SendMessageAsync(list);
		}
		else
		{
			message.Channel.SendMessageAsync("There are no active games.");
		}
	}

	public override string LogString => "listed current games";
	public override string[] Aliases => new[] { "activegames", "activegame", "active", "actives", "ag", "listactivegames" };
}

internal class ClearGamesCommand : Command
{
	private int _count;

	public override void Execute()
	{
		_count = Program.GameHandler.ActiveGames.Count;
		while (Program.GameHandler.ActiveGames.Count > 0)
		{
			Program.GameHandler.ActiveGames[0].Shutdown();
			Program.GameHandler.EndGame(Program.GameHandler.ActiveGames[0]);
		}
	}

	public override string LogString => $"shutting down {_count} active game(s).";
	public override string[] Aliases => new[] { "cleargames", "endgames" };
	public override ulong[] RequiredRoles => new[] { Program.Config.ModRoleID ?? 0 };
}
