namespace WingTechBot;

internal class SpamCommand : Command
{
	public override void Execute()
	{
		message.Channel.SendMessageAsync($"Updating running karma.");
		Program.KarmaHandler.ClearRunningKarma();
	}

	public override string LogString => $"manually updating running karma.";
	public override bool OwnerOnly => true;
	public override bool Audit => true;
}
