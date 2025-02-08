namespace WingTechBot.Commands.Reactions;

public sealed partial class ReactionsGivenCommand : SlashCommand
{
	public override async Task SetUp(WingTechBot bot)
	{
		await AddCommand(bot, new SlashCommandBuilder()
			.WithName("reactions-given")
			.WithDescription("Shows all reactions you have given this year to a user or role (excluding legacy karma)")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("mentionable")
				.WithDescription("The user or role")
				.WithType(ApplicationCommandOptionType.Mentionable)
				.WithRequired(false)
			)
		);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		await ReactionsGivenToMentionableForYear(command);
	}
}