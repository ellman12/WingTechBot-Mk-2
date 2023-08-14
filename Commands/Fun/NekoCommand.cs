namespace WingTechBot;

internal class NekoCommand : Command
{
	public override void Execute() => message.Channel.SendFileAsync($@"Images/neko{Program.Random.Next(5)}.png");

	public override string LogString => "posted smexy cat boy";
	public override string[] Aliases => new[] { "neko", "catboy", "catboi", "catgirl" };
}
