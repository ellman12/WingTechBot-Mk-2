namespace WingTechBot.Commands.Reactions;

public sealed partial class ReactionsCommand : SlashCommand
{
	public override async Task SetUp(WingTechBot bot)
	{
		var reactionsCommand = new SlashCommandBuilder()
			.WithName("reactions")
			.WithDescription("Shows totals for all reactions you have received this year")
		;
		await AddCommand(bot, reactionsCommand);
	}
	
	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		var options = command.Data.Options;

		if (options.Count == 0)
		{
			await UserReactionsForYear(command);
			return;
		}
	}
}