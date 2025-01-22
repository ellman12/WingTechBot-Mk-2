namespace ModelTests.ReactionTests;

[TestFixture]
public sealed class RemoveAllReactionsTests : ModelTests
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
		int expectedReactionRows = wtbMessages * reactionsPerMessage;
		
		//Add reactions
		const ulong GiverId = 123;
		const ulong ReceiverId = 789;
		ulong messageId = 450;

		foreach (int _ in Enumerable.Range(0, wtbMessages))
		{
			foreach (int e in Enumerable.Range(0, reactionsPerMessage))
			{
				var emote = ValidReactionEmotes[e];
				await Reaction.AddReaction(GiverId, ReceiverId, messageId, emote.Name, emote.DiscordEmoteId);

				var newEmote = await ReactionEmote.Find(emote.Name, emote.DiscordEmoteId);
				var reaction = await Reaction.Find(GiverId, ReceiverId, messageId, newEmote.Id);
				Assert.NotNull(newEmote);
				Assert.NotNull(reaction);
			}

			messageId++;
		}

		await using BotDbContext context = new();
		Assert.AreEqual(await context.Reactions.CountAsync(), expectedReactionRows);

		await Reaction.RemoveAllReactions(450);
		Assert.False(await context.Reactions.AnyAsync(r => r.MessageId == 450));
	}

	[TestCase(1, 1), TestCase(2, 1), TestCase(1, 2), TestCase(2, 2), TestCase(3, 2), TestCase(4, 4)]
	public async Task FailsWhenReactionsDontExist(int wtbMessages, int emotesPerMessage)
	{
		await ReactionsExist(wtbMessages, emotesPerMessage);

		Assert.ThrowsAsync<ArgumentException>(async () => await Reaction.RemoveAllReactions(69));
	}
}