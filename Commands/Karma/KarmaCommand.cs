namespace WingTechBot;

internal class KarmaCommand : Command
{
	public override void Execute()
	{
		if (Program.KarmaHandler.KarmaDictionary.ContainsKey(requested.Id))
		{
			var counts = Program.KarmaHandler.KarmaDictionary[requested.Id];

			message.Channel.SendMessageAsync($"{requested.Mention} has {counts[0] - counts[1]} karma on record. (<:upvote:672248776903098369> {counts[0]} <:downvote:672248822474211334> {counts[1]})");
		}
		else
		{
			message.Channel.SendMessageAsync($"{requested.Mention} does not seem to have any karma on record.");
		}
	}

	public override string LogString => $"reported {requested}'s karma";
	public override bool GetRequested => true;
	public override string[] Aliases => new[] { "karma", "k" };
}
