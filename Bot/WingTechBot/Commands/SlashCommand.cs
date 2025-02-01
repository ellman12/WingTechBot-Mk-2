namespace WingTechBot.Commands;

///Represents any kind of slash command.
public abstract class SlashCommand
{
	public abstract Task SetUp(WingTechBot bot);

	protected string Name { get; private set; }

	protected WingTechBot Bot { get; set; }

	///<summary>If no slash commands have changed, pass --no-recreate-commands to skip that step in initialization.</summary>
	///<remarks>This process takes a very long time to run when the bot starts, so don't recreate them unless needed.</remarks>
	public static bool NoRecreateCommands { get; set; }

	protected async Task AddCommand(WingTechBot bot, SlashCommandBuilder commandBuilder)
	{
		Bot = bot;
		Name = commandBuilder.Name;
		Bot.Client.SlashCommandExecuted += HandleCommand;

		if (NoRecreateCommands)
			return;

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