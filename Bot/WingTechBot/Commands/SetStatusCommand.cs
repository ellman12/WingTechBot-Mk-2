namespace WingTechBot.Commands;

///Used to set the status of <see cref="WingTechBot"/>.
public sealed class SetStatusCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("set-status")
			.WithDescription("Sets the status of the bot")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("status")
				.WithDescription("The new status of the bot")
				.WithType(ApplicationCommandOptionType.String)
				.WithRequired(true)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		var status = (string)command.Data.Options.Single(o => o.Name == "status");
		await Bot.Client.SetCustomStatusAsync(status);

		await command.DeleteOriginalResponseAsync();
	}
}