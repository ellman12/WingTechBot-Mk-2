namespace WingTechBot.Commands;

///Represents any kind of slash command.
public abstract class SlashCommand
{
	public abstract Task SetUp(WingTechBot bot);

	public abstract Task HandleCommand(SocketSlashCommand command);

	public WingTechBot Bot { get; protected set; }
}
