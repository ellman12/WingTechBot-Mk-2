namespace WingTechBot.Commands;

///Represents any kind of slash command.
public abstract class SlashCommand
{
	public abstract Task SetUp(WingTechBot bot);

	protected string Name { get; private set; }

	protected WingTechBot Bot { get; set; }

	protected async Task AddCommand(WingTechBot bot, SlashCommandBuilder commandBuilder)
	{
		Bot = bot;
		Name = commandBuilder.Name;
		Bot.Client.SlashCommandExecuted += HandleCommand;

		try
		{
			await Bot.Client.CreateGlobalApplicationCommandAsync(commandBuilder.Build());
		}
		catch (Exception e)
		{
			await Logger.LogExceptionAsMessage(e, Bot.BotChannel);
		}
	}

	public abstract Task HandleCommand(SocketSlashCommand command);
}