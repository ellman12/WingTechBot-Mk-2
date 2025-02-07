namespace WingTechBot.Commands.Reactions;

///Works with <see cref="ReactionTracker"/> to facilitate scolding those who give themselves certain reactions like upvote and awards.
public static class ReactionScolding
{
	private static readonly string[] upvoteScolds =
	[
		"god imagine upvoting yourself",
		"eww, a self-upvote",
		"upvoting yourself? cringe",
		"eww don't upvote yourself, this isn't reddit",
		"i'm going to verbally harass you if you keep upvoting yourself",
		"smh my head this man just self-upvoted",
		"gross self-upvote",
		"redditor",
		"you know upvoting yourself doesn't increase your karma, right?",
		"i'm telling ben you upvoted yourself",
		"upvoting yourself? not cool",
		"peepee poopoo don't upvote yourself",
		"only nerds upvote themselves",
	];

	private static readonly string[] awardScolds =
	[
		"really out here giving yourself an award, are ya?",
		"get a load of this guy giving themselves an award 👉",
		"How you look giving yourself an award:\n[img](https://user-images.githubusercontent.com/14880945/104736592-80303380-5743-11eb-8224-2bae4fab6f15.png) \n", //Obama meme
	];

	private static readonly Dictionary<string, string[]> scolds = new()
	{
		{"upvote", upvoteScolds},
		{"silver", awardScolds},
		{"gold", awardScolds},
		{"platinum", awardScolds},
	};

	public static async Task SendScold(string emoteName, IMessageChannel channel, IUser recipient)
	{
		if (!scolds.TryGetValue(emoteName, out var group))
			return;

		string message = group[Random.Shared.Next(0, group.Length)];
		await channel.SendMessageAsync($"{message} {recipient.Mention}");
	}
}