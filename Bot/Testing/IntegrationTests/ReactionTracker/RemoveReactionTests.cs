namespace IntegrationTests.ReactionTracker;

[TestFixture]
public sealed class RemoveReactionTests : ReactionTrackerTests
{
	[TestCase(1, 1, 1, 1)]
	[TestCase(2, 1, 1, 2)]
	[TestCase(2, 2, 2, 2 * 2)]
	[TestCase(8, 4, 4, 8 * 4)]
	public async Task RemoveReactionsFromMessage(int wtbMessages, int reactionsPerMessage, int expectedEmoteRows, int expectedReactionRows)
	{
		var messages = await CreateMessages(wtbMessages, reactionsPerMessage);

		//Ensure database tasks finish.
		await Task.Delay(300);

		await using BotDbContext context = new();
		Assert.AreEqual(await context.ReactionEmotes.CountAsync(), expectedEmoteRows);
		Assert.AreEqual(await context.Reactions.CountAsync(), expectedReactionRows);
		Assert.That(await context.Reactions.AllAsync(r => r.GiverId == BotTester.Config.UserId && r.ReceiverId == WingTechBot.Config.UserId));

		//Remove the freshly-created reactions
		foreach (int i in Enumerable.Range(0, wtbMessages))
		{
			foreach (int rpm in Enumerable.Range(0, reactionsPerMessage))
			{
				var message = await BotTester.BotChannel.GetMessageAsync(messages[i].Id);
				await message.RemoveReactionAsync(ReactionEmotes[rpm].Parsed, BotTester.Config.UserId);
				expectedReactionRows--;

				await Task.Delay(200);

				Assert.AreEqual(await context.ReactionEmotes.CountAsync(), expectedEmoteRows);
				Assert.AreEqual(await context.Reactions.CountAsync(), expectedReactionRows);
			}
		}
	}
}