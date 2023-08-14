namespace WingTechBot;

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
