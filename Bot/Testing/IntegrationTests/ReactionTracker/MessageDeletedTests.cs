namespace IntegrationTests.ReactionTracker;

public sealed class MessageDeletedTests : ReactionTrackerTests
{
	[TestCase(1, 1), TestCase(2, 1), TestCase(2, 2), TestCase(8, 4)]
	public async Task MessageDeleted(int wtbMessages, int reactionsPerMessage)
	{
		int expectedReactionRows = (wtbMessages * reactionsPerMessage) + wtbMessages; //Include self-downvotes

		var messages = await CreateMessages(wtbMessages, reactionsPerMessage);

		await Task.Delay(Constants.IntegrationTestDelay);

		await using BotDbContext context = new();
		Assert.AreEqual(await context.ReactionEmotes.CountAsync(), reactionsPerMessage);
		Assert.AreEqual(await context.Reactions.CountAsync(), expectedReactionRows);
		Assert.That(await context.Reactions.AllAsync(r => r.ReceiverId == WingTechBot.Config.UserId));

		var message = await BotTester.BotChannel.GetMessageAsync(messages.First().Id);
		await message.DeleteAsync();

		await Task.Delay(Constants.IntegrationTestDelay);

		Assert.AreEqual(await context.Reactions.CountAsync(), expectedReactionRows - (reactionsPerMessage + 1));
		Assert.That(await context.Reactions.AllAsync(r => r.MessageId != message.Id));
	}
}