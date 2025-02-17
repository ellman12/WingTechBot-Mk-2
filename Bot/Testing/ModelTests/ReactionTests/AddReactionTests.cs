namespace ModelTests.ReactionTests;

[TestFixture]
public sealed class AddReactionTests : ReactionTests
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
	public async Task ReactionEmoteDoesNotExist(ulong giverId, ulong receiverId, ulong channelId, ulong messageId, string emoteName, ulong? discordEmoteId)
	{
		await Reaction.AddReaction(giverId, receiverId, channelId, messageId, emoteName, discordEmoteId);
		var emote = await ReactionEmote.Find(emoteName, discordEmoteId);
		Assert.NotNull(emote);

		var reaction = await Reaction.Find(giverId, receiverId, channelId, messageId, emote.Id);
		Assert.NotNull(reaction);
		
		Assert.True(reaction.Emote.Name == emote.Name);
	}

	[TestCaseSource(nameof(ValidReactions))]
	public async Task ReactionEmoteExists(ulong giverId, ulong receiverId, ulong channelId, ulong messageId, string emoteName, ulong? discordEmoteId)
	{
		await ReactionEmote.AddEmote(emoteName, discordEmoteId);
		var emote = await ReactionEmote.Find(emoteName, discordEmoteId);
		Assert.NotNull(emote);

		await Reaction.AddReaction(giverId, receiverId, channelId, messageId, emoteName, discordEmoteId);
		var reaction = await Reaction.Find(giverId, receiverId, channelId, messageId, emote.Id);
		Assert.NotNull(reaction);
		
		Assert.True(reaction.Emote.Name == emote.Name);
	}
	
	[TestCaseSource(nameof(ValidReactions))]
	public async Task FailsWhenReactionExists(ulong giverId, ulong receiverId, ulong channelId, ulong messageId, string emoteName, ulong? discordEmoteId)
	{
		await Reaction.AddReaction(giverId, receiverId, channelId, messageId, emoteName, discordEmoteId);

		var emote = await ReactionEmote.Find(emoteName, discordEmoteId);
		Assert.NotNull(emote);

		var reaction = await Reaction.Find(giverId, receiverId, channelId, messageId, emote.Id);
		Assert.NotNull(reaction);

		Assert.ThrowsAsync<ArgumentException>(async () => await Reaction.AddReaction(giverId, receiverId, channelId, messageId, emoteName, discordEmoteId));
	}

	[Test, TestCaseSource(nameof(InvalidReactions))]
	public void FailsForInvalidReactions(ulong giverId, ulong receiverId, ulong channelId, ulong messageId, string emoteName, ulong? discordEmoteId)
	{
		Assert.ThrowsAsync<ArgumentException>(async () => await Reaction.AddReaction(giverId, receiverId, channelId, messageId, emoteName, discordEmoteId));
	}
}