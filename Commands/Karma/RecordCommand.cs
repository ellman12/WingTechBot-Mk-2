namespace WingTechBot;

internal class RecordCommand : Command
{
	public override void Execute()
	{
		if (Program.KarmaHandler.KarmaDictionary.ContainsKey(requested.Id))
		{
			var counts = Program.KarmaHandler.KarmaDictionary[requested.Id];

			message.Channel.SendMessageAsync($"{requested.Mention} has {counts[0] - counts[1]} karma on record. (<:upvote:672248776903098369> {counts[0]} <:downvote:672248822474211334> {counts[1]}) {requested} has: {counts[2]} <:silver:672249246442979338> {counts[3]} <:gold:672249212322316308> {counts[4]} <:platinum:672249164846858261>");
		}
		else
		{
			message.Channel.SendMessageAsync($"{requested.Mention} does not seem to have any awards or karma on record.");
		}
	}

	public override string LogString => $"reported {requested}'s record";
	public override bool GetRequested => true;
	public override string[] Aliases => new[] { "record", "records", "r" };
}
