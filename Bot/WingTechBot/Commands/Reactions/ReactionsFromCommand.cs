namespace WingTechBot.Commands.Reactions;

public sealed class ReactionsFromCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("reactions-from")
			.WithDescription("Shows all reactions you have received this year from a user or role (excluding legacy karma)")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("mentionable")
				.WithDescription("The user or role")
				.WithType(ApplicationCommandOptionType.Mentionable)
				.WithRequired(true)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		await ReactionsFromMentionableForYear(command);
	}

	private async Task ReactionsFromMentionableForYear(SocketSlashCommand command)
	{
		int year = DateTime.Now.Year;
		ulong receiverId = command.User.Id;
		string name = "";
		Dictionary<ReactionEmote, int> reactions = new();

		var mentionable = (IMentionable)command.Data.Options.First(o => o.Name == "mentionable").Value;
		if (mentionable is SocketRole role)
		{
			if (role.IsEveryone)
			{
				reactions = await Reaction.GetReactionsUserReceived(receiverId, year);
				name = role.Name[1..];
			}
			else
			{
				var usersWithRole = (await Bot.Guild.GetUsersAsync().FlattenAsync()).Where(u => u.RoleIds.Any(id => id == role.Id));
				reactions = await Reaction.GetReactionsFromUsersForYear(usersWithRole.Select(u => u.Id).ToArray(), receiverId, year);
				name = role.Name;
			}
		}
		else if (mentionable is SocketUser user)
		{
			reactions = await Reaction.GetReactionsFromUsersForYear([user.Id], receiverId, year);
			name = user.Username;
		}

		string message;
		if (reactions.Count > 0)
		{
			message = reactions.Aggregate($"{command.User.Username} received\n", (current, reaction) => current + $"* {reaction.Value} {reaction.Key}\n");
			message += $"from {name} in {year}";
		}
		else
		{
			message = $"No reactions received from {name} for {year}";
		}

		await command.FollowupAsync(message);
	}
}