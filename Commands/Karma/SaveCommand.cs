namespace WingTechBot;

internal class SaveCommand : Command
{
	public override void Execute()
	{
		Program.KarmaHandler.Save();
		message.Channel.SendMessageAsync($"Saving karma values.");
	}

	public override string LogString => "manually saved";
	public override bool Audit => true;
	public override bool OwnerOnly => true;
}
