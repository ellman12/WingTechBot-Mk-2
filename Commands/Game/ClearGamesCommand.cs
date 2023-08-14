namespace WingTechBot;

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
