namespace WingTechBot.Commands;

///Represents any kind of slash command.
public abstract class SlashCommand
{
	public abstract Task SetUp(WingTechBot bot);

	public abstract Task HandleCommand(SocketSlashCommand command);

	public WingTechBot Bot { get; protected set; }

	protected async Task PreprocessCommand(SocketSlashCommand command)
	{
		//Makes command timeout longer than 3 seconds. Essential for debug breakpoints.
		await command.DeferAsync();
	}
}
