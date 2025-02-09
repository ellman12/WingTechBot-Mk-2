namespace WingTechBot.Commands.Reactions;

public sealed class ReactionsGivenCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("reactions-given")
			.WithDescription("Shows all reactions you have given this year to a user or role (excluding legacy karma)")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("mentionable")
				.WithDescription("The user or role")
				.WithType(ApplicationCommandOptionType.Mentionable)
				.WithRequired(false)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		await ReactionsGivenToMentionableForYear(command);
	}

	private async Task ReactionsGivenToMentionableForYear(SocketSlashCommand command)
	{
		int year = DateTime.Now.Year;
		ulong giverId = command.User.Id;
		string name = "";
		Dictionary<ReactionEmote, int> reactions = new();

		var mentionable = (IMentionable)command.Data.Options.FirstOrDefault(o => o.Name == "mentionable")?.Value;
		if (mentionable == null)
		{
			reactions = await Reaction.GetReactionsUserGiven(giverId, year);
			name = "everyone";
		}
		else if (mentionable is SocketRole role)
		{
			if (role.IsEveryone)
			{
				reactions = await Reaction.GetReactionsUserGiven(giverId, year);
				name = "everyone";
			}
			else
			{
				var usersWithRole = (await Bot.Guild.GetUsersAsync().FlattenAsync()).Where(u => u.RoleIds.Any(id => id == role.Id));
				reactions = await Reaction.GetReactionsGivenToUsersForYear(giverId, usersWithRole.Select(u => u.Id).ToArray(), year);
				name = role.Name;
			}
		}
		else if (mentionable is SocketUser user)
		{
			reactions = await Reaction.GetReactionsGivenToUsersForYear(giverId, [user.Id], year);
			name = user.Username;
		}

		string message;
		if (reactions.Count > 0)
		{
			message = reactions.Aggregate($"{command.User.Username} gave\n", (current, reaction) => current + $"* {reaction.Value} {reaction.Key}\n");
			message += $"to {name} in {year}";
		}
		else
		{
			message = $"No reactions given to {name} for {year}";
		}

		await command.FollowupAsync(message);
	}
}