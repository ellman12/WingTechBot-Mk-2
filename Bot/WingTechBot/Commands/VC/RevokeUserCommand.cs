namespace WingTechBot.Commands.VC;

public sealed class RevokeUserCommand : SlashCommand
{
	public override bool Admin => true;

	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("revoke-user")
			.WithDescription("Revoke a user from the WTB soundboard website")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("user")
				.WithDescription("The user to revoke")
				.WithType(ApplicationCommandOptionType.User)
				.WithRequired(true)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.Data.Options.SingleOrDefault(o => o.Name == "user")?.Value is not SocketUser user)
			throw new NullReferenceException("Provide a user");

		await SoundboardUser.RevokeUser(user.Id);
		await command.FollowupAsync($"{user.Username} has been revoked");
	}
}