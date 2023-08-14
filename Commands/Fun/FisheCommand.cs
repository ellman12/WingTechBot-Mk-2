namespace WingTechBot;

internal class FisheCommand : Command
{
	public override void Execute() => message.Channel.SendFileAsync(@"Images/fishe.jpg");

	public override string LogString => "posted fishe";
}
