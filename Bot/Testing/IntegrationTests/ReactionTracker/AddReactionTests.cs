using Discord;

namespace IntegrationTests.ReactionTracker;

[TestFixture]
public sealed class AddReactionTests : ReactionTrackerTests
{
	[TestCase(1, 1, 1, 1)]
	[TestCase(2, 1, 1, 2)]
	[TestCase(2, 2, 2, 2 * 2)]
	[TestCase(8, 4, 4, 8 * 4)]
	public async Task AddReactionsToMessages(int wtbMessages, int reactionsPerMessage, int expectedEmoteRows, int expectedReactionRows)
	{
		await AddMessagesAndReactions(wtbMessages, reactionsPerMessage);
		
		await Task.Delay(300); //Ensure DB is finished updating.
		
		await using BotDbContext context = new();
		Assert.AreEqual(await context.ReactionEmotes.CountAsync(), expectedEmoteRows);
		Assert.AreEqual(await context.Reactions.CountAsync(), expectedReactionRows);
		Assert.That(await context.Reactions.AllAsync(r => r.GiverId == BotTester.Config.UserId && r.ReceiverId == WingTechBot.Config.UserId));
	}
}
