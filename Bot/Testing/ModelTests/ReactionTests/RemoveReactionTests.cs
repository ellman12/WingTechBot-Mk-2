namespace ModelTests.ReactionTests;

[TestFixture]
public sealed class RemoveReactionTests : ReactionTests
{
	private static readonly TestCaseData[] ValidReactions =
	[
		new(123ul, 456ul, 789ul, 867ul, "🤩", null),
		new(123ul, 456ul, 789ul, 867ul, "upvote", 8947589432758943ul),
		new(123ul, 456ul, 789ul, 867ul, "👌", null),
		new(123ul, 456ul, 789ul, 867ul, "👌🏿", null),
		new(123ul, 456ul, 789ul, 867ul, "🤷‍♂️", null),
		new(123ul, 456ul, 789ul, 867ul, "🤷🏿‍♂️", null)
	];

	private static readonly TestCaseData[] InvalidReactions =
	[
		new(123ul, 456ul, 789ul, 867ul, "", null),
		new(123ul, 745849ul, 0ul, 867ul, "🤩", null),
		new(123ul, 456ul, 789ul, 867ul, "upvote", 0ul),
		new(4445ul, 4445ul, 789ul, 867ul, "upvote", 0ul),
		new(123ul, 456ul, 789ul, 867ul, "🤷‍♂️", 69420ul),
		new(123ul, 456ul, 789ul, 867ul, "👌👌🏼", null),
		new(0ul, 456ul, 789ul, 867ul, "🤷🏿‍♂️", null)
	];


	[TestCaseSource(nameof(ValidReactions))]
	public async Task ReactionExists(ulong giverId, ulong receiverId, ulong channelId, ulong messageId, string emoteName, ulong? discordEmoteId)
	{
		foreach (int i in Enumerable.Range(1, 4))
		{
			messageId++;
			await Reaction.AddReaction(giverId, receiverId, channelId, messageId, emoteName, discordEmoteId);
			var emote = await ReactionEmote.Find(emoteName, discordEmoteId);
			var reaction = await Reaction.Find(giverId, receiverId, channelId, messageId, emote.Id);
			Assert.NotNull(emote);
			Assert.NotNull(reaction);
		}

		await using BotDbContext context = new();
		Assert.True(context.Reactions.Count() == 4);
		Assert.True(context.Reactions.OrderBy(r => r.MessageId).Last().MessageId == messageId);

		await Reaction.RemoveReaction(giverId, receiverId, channelId, messageId, emoteName, discordEmoteId);
		Assert.True(context.Reactions.Count() == 3);
		Assert.True(context.Reactions.OrderBy(r => r.MessageId).Last().MessageId == messageId - 1);	
	}
	
	[TestCaseSource(nameof(ValidReactions))]
	public async Task FailsWhenReactionDoesntExist(ulong giverId, ulong receiverId, ulong channelId, ulong messageId, string emoteName, ulong? discordEmoteId)
	{
		await using BotDbContext context = new();
		Assert.IsEmpty(context.Reactions);

		Assert.ThrowsAsync<ArgumentException>(async () => await Reaction.RemoveReaction(giverId, receiverId, channelId, messageId, emoteName, discordEmoteId));
	}
	
	[TestCaseSource(nameof(InvalidReactions))]
	public async Task FailsForInvalidReactions(ulong giverId, ulong receiverId, ulong channelId, ulong messageId, string emoteName, ulong? discordEmoteId)
	{
		await using BotDbContext context = new();
		Assert.IsEmpty(context.Reactions);

		Assert.ThrowsAsync<ArgumentException>(async () => await Reaction.RemoveReaction(giverId, receiverId, channelId, messageId, emoteName, discordEmoteId));
	}
}