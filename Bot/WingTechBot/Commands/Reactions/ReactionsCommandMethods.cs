namespace WingTechBot.Commands.Reactions;

public sealed partial class ReactionsCommand
{
	private static async Task UserReactionsForYear(SocketSlashCommand command)
	{
		int year = DateTime.Now.Year;
		var reactions = await Reaction.GetReactionsUserReceived(command.User.Id, year);

		string message;
		if (reactions.Length > 0)
		{
			message = reactions.Aggregate($"{command.User.Username} received\n", (current, reaction) => current + $"* {reaction.count} {reaction.reactionEmote}\n");
			message += $"in {year}";
		}
		else
		{
			message = $"No reactions received for {year}";
		}

		await command.FollowupAsync(message);
	}

}