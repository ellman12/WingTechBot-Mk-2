namespace WingTechBot.Commands.Reactions;

public sealed class RecordCommand : SlashCommand
{
	private static readonly HashSet<string> emoteNames = ["upvote", "downvote", "silver", "gold", "platinum"];
	private static Dictionary<ReactionEmote, int> serverEmotes;

	protected override SlashCommandBuilder CreateCommand()
	{
		serverEmotes = Bot.Guild.GetEmotesAsync().Result
			.Where(e => emoteNames.Contains(e.Name))
			.Select(e => new ReactionEmote(e.Name, e.Id))
			.ToDictionary(e => e, _ => 0);

		return new SlashCommandBuilder()
			.WithName("record")
			.WithDescription("Shows your upvotes, downvotes, and awards.");
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		await GetUserRecord(command);
	}

	private async Task GetUserRecord(SocketSlashCommand command)
	{
		int year = DateTime.Now.Year;
		var reactions = await Reaction.GetReactionsUserReceived(command.User.Id, year);

		var result = serverEmotes
			.Where(r => emoteNames.Contains(r.Key.Name))
			.GroupJoin(reactions, s => s.Key.Name, r => r.Key.Name, (pair, pairs) => new
			{
				Emote = pair.Key,
				Count = pair.Value + pairs.DefaultIfEmpty().Single().Value
			})
			.ToDictionary(e => e.Emote.Name, e => (e.Emote, e.Count));

		var upvote = result["upvote"];
		var downvote = result["downvote"];
		var silver = result["silver"];
		var gold = result["gold"];
		var platinum = result["platinum"];

		int karma = upvote.Count - downvote.Count;
		await command.FollowupAsync($"{command.User.Mention} has {karma} karma ({upvote.Count} {upvote.Emote} {downvote.Count} {downvote.Emote}), and {silver.Count} {silver.Emote} {gold.Count} {gold.Emote} {platinum.Count} {platinum.Emote} for {year}");
	}
}