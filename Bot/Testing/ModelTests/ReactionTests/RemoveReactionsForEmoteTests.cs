namespace ModelTests.ReactionTests;

public sealed class RemoveReactionsForEmoteTests : ReactionTests
{
	private static readonly ReactionEmote[] ValidReactionEmotes =
	[
		new("🤩", null),
		new("upvote", 123456),
		new("👎🏼", null),
		new("👨🏾‍💻", null),
	];
	
	[TestCase(1, 1), TestCase(2, 1), TestCase(1, 2), TestCase(2, 2), TestCase(3, 2), TestCase(4, 4)]
	public async Task ReactionsExist(int wtbMessages, int reactionsPerMessage)
	{
		const int MessageId = 450;
		int expectedReactionRows = wtbMessages * reactionsPerMessage;

		await CreateReactions(wtbMessages, reactionsPerMessage, ValidReactionEmotes, 123, 789, 867, MessageId);
		
		await Task.Delay(Constants.ModelTestDelay);
		
		await using BotDbContext context = new();
		Assert.AreEqual(await context.Reactions.CountAsync(), expectedReactionRows);

		var emote = ValidReactionEmotes.First();
		await Reaction.RemoveReactionsForEmote(MessageId, emote.Name, emote.DiscordEmoteId);
		Assert.False(await context.Reactions.AnyAsync(r => r.MessageId == MessageId && r.Emote.Name == emote.Name && r.Emote.DiscordEmoteId == emote.DiscordEmoteId));
	}

	[TestCase(1, 1), TestCase(2, 1), TestCase(1, 2), TestCase(2, 2), TestCase(3, 2), TestCase(4, 4)]
	public async Task FailsWhenReactionsDontExist(int wtbMessages, int emotesPerMessage)
	{
		await ReactionsExist(wtbMessages, emotesPerMessage);

		Assert.ThrowsAsync<ArgumentException>(async () => await Reaction.RemoveReactionsForEmote(69, "cheese", 69420)); //These last two values don't matter
	}
}