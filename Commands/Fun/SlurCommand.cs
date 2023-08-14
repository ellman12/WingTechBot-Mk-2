namespace WingTechBot;

internal class SlurCommand : Command
{
	public override void Execute() => message.Channel.SendMessageAsync($"Bad {message.Author.Mention}");

	public override string LogString => $"scolded {message.Author}";
}
