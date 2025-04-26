namespace WingTechBot.Commands.VC;

public sealed class AuthenticateUserCommand : SlashCommand
{
	public override bool Admin => true;

	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("authenticate-user")
			.WithDescription("Authenticate a user to use the WTB soundboard website")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("user")
				.WithDescription("The user to authenticate")
				.WithType(ApplicationCommandOptionType.User)
				.WithRequired(true)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.Data.Options.SingleOrDefault(o => o.Name == "user")?.Value is not SocketUser user)
			throw new NullReferenceException("Provide a user");

		await SoundboardUser.AuthenticateUser(user.Id, command.User.Id);
		await command.FollowupAsync($"{user.Username} has been authenticated");
	}
}