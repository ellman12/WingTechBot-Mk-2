namespace IntegrationTests.ReactionTracker;

public sealed class RemoveReactionsForEmoteTests : ReactionTrackerTests
{
	[TestCase(1, 1), TestCase(2, 1), TestCase(2, 2), TestCase(8, 4)]
	public async Task RemoveReactionsForEmote(int wtbMessages, int reactionsPerMessage)
	{
		int expectedReactionRows = (wtbMessages * reactionsPerMessage) + wtbMessages; //Include self-downvotes
		var messages = await CreateMessages(wtbMessages, reactionsPerMessage);

		//Ensure we can properly remove multiple reaction rows.
		var message = await WingTechBot.BotChannel.GetMessageAsync(messages.First().Id);
		await message.AddReactionAsync(ReactionEmotes.Last().Parsed);
		expectedReactionRows++;

		await Task.Delay(Constants.DatabaseDelay);

		await using BotDbContext context = new();
		Assert.AreEqual(await context.Reactions.CountAsync(), expectedReactionRows);

		var firstReaction = (await WingTechBot.BotChannel.GetMessageAsync(messages.First().Id)).Reactions.First();
		await messages.First().RemoveAllReactionsForEmoteAsync(firstReaction.Key);

		await Task.Delay(Constants.DatabaseDelay);
		
		Assert.AreEqual(await context.Reactions.CountAsync(), expectedReactionRows - firstReaction.Value.ReactionCount);
	}
}