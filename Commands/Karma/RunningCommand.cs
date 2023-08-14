namespace WingTechBot;

internal class RunningCommand : Command
{
	public override void Execute()
	{
		if (Program.KarmaHandler.RunningKarma.ContainsKey(requested.Id))
		{
			var counts = Program.KarmaHandler.RunningKarma[requested.Id];

			message.Channel.SendMessageAsync($"{requested.Mention} has {counts[0] - counts[1]} running karma on record. (<:upvote:672248776903098369> {counts[0]} <:downvote:672248822474211334> {counts[1]})");
		}
		else
		{
			message.Channel.SendMessageAsync($"{requested.Mention} does not seem to have any running karma right now.");
		}
	}

	public override string LogString => $"reported {requested}'s running karma";
	public override bool GetRequested => true;
	public override ulong[] RequiredRoles => new[] { Program.Config.ModRoleID ?? 0 };
}
