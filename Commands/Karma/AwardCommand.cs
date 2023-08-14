namespace WingTechBot;

internal class AwardCommand : Command
{
	public override void Execute()
	{
		if (Program.KarmaHandler.KarmaDictionary.ContainsKey(requested.Id))
		{
			var counts = Program.KarmaHandler.KarmaDictionary[requested.Id];

			message.Channel.SendMessageAsync($"{requested.Mention} has {counts[2]} <:silver:672249246442979338> {counts[3]} <:gold:672249212322316308> {counts[4]} <:platinum:672249164846858261>");
		}
		else
		{
			message.Channel.SendMessageAsync($"{requested.Mention} does not seem to have any awards on record.");
		}
	}

	public override string LogString => $"reported {requested}'s awards";
	public override bool GetRequested => true;
	public override string[] Aliases => new[] { "award", "awards", "a" };
}
