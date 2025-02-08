namespace WingTechBot.Commands.Reactions;

public sealed partial class ReactionsFromCommand : SlashCommand
{
	public override async Task SetUp(WingTechBot bot)
	{
		await AddCommand(bot, new SlashCommandBuilder()
			.WithName("reactions-from")
			.WithDescription("Shows all reactions you have received this year from a user or role (excluding legacy karma)")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("mentionable")
				.WithDescription("The user or role")
				.WithType(ApplicationCommandOptionType.Mentionable)
				.WithRequired(true)
			)
		);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		await ReactionsFromMentionableForYear(command);
	}
}