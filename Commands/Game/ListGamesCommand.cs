namespace WingTechBot;

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
