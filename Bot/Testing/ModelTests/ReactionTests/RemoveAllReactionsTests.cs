namespace ModelTests.ReactionTests;

[TestFixture]
public sealed class RemoveAllReactionsTests : ReactionTests
{
	private static readonly ReactionEmote[] ValidReactionEmotes =
	[
		new("🤩", null),
		new("upvote", 123456),
		new("🙅🏼‍♂️", null),
		new("👨🏾‍💻", null),
	];

	[TestCase(1, 1), TestCase(2, 1), TestCase(1, 2), TestCase(2, 2), TestCase(3, 2), TestCase(4, 4)]
	public async Task ReactionsExist(int wtbMessages, int reactionsPerMessage)
	{
		const int MessageId = 450;
		int expectedReactionRows = wtbMessages * reactionsPerMessage;

		await CreateReactions(wtbMessages, reactionsPerMessage, ValidReactionEmotes, 123, 789, 857, MessageId);

		await Task.Delay(Constants.ModelTestDelay);
		
		await using BotDbContext context = new();
		Assert.AreEqual(await context.Reactions.CountAsync(), expectedReactionRows);

		await Reaction.RemoveAllReactions(MessageId);
		Assert.False(await context.Reactions.AnyAsync(r => r.MessageId == MessageId));
	}

	[TestCase(1, 1), TestCase(2, 1), TestCase(1, 2), TestCase(2, 2), TestCase(3, 2), TestCase(4, 4)]
	public async Task FailsWhenReactionsDontExist(int wtbMessages, int emotesPerMessage)
	{
		await ReactionsExist(wtbMessages, emotesPerMessage);

		Assert.ThrowsAsync<ArgumentException>(async () => await Reaction.RemoveAllReactions(69));
	}
}