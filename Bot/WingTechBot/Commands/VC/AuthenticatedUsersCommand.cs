namespace WingTechBot.Commands.VC;

public sealed class AuthenticatedUsersCommand : SlashCommand
{
	public override bool Admin => true;

	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("authenticated-users")
			.WithDescription("Lists users who can use the WTB soundboard website");
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		var allUsers = await SoundboardUser.GetAll();
		if (allUsers.Any())
		{
			var users = allUsers.Join(Bot.Guild.Users, soundboardUser => soundboardUser.Id, guildUser => guildUser.Id, (soundboardUser, guildUser) => new
			{
				guildUser.Username,
			});

			string output = users.Aggregate("Authenticated users:\n", (current, user) => current + $"{user.Username}\n");
			await command.FollowupAsync(output);
		}
		else
		{
			await command.FollowupAsync("No soundboard users");
		}
	}
}