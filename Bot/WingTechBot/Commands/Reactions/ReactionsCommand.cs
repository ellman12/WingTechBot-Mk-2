namespace WingTechBot.Commands.Reactions;

public sealed partial class ReactionsCommand : SlashCommand
{
	public override async Task SetUp(WingTechBot bot)
	{
		Bot = bot;
		Bot.Client.SlashCommandExecuted += HandleCommand;

		var reactionsCommand = new SlashCommandBuilder()
			.WithName("reactions")
			.WithDescription("Shows totals for all reactions you have received this year")
		;

		try
		{
			await Bot.Client.CreateGlobalApplicationCommandAsync(reactionsCommand.Build());
		}
		catch (Exception e)
		{
			Logger.LogLine("Error adding reactions command");
			Logger.LogException(e);
		}
	}
	
	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != "reactions")
			return;

		var options = command.Data.Options;

		if (options.Count == 0)
		{
			await UserReactionsForYear(command);
			return;
		}
	}
}