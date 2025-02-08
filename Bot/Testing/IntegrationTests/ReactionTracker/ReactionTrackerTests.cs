namespace IntegrationTests.ReactionTracker;

public abstract class ReactionTrackerTests : IntegrationTests
{
	public static readonly ReactionEmote[] ReactionEmotes =
	[
		new("downvote", 1328082903703224330ul),
		new("upvote", 1325537736626671697ul),
		new("👀", null),
		new("🤩", null)
	];

	[SetUp]
	public async Task ReactionTrackerSetup()
	{
		await WingTechBot.BotChannel.SendMessageAsync($"Begin test {TestContext.CurrentContext.Test.Name}");
	}

	[TearDown]
	public async Task ReactionTrackerTearDown()
	{
		await WingTechBot.BotChannel.SendMessageAsync($"Finish test {TestContext.CurrentContext.Test.Name}");
	}

	protected static async Task<List<IMessage>> CreateMessages(int wtbMessages, int reactionsPerMessage)
	{
		List<IMessage> messages = [];

		foreach (int i in Enumerable.Range(0, wtbMessages))
		{
			messages.Add(await WingTechBot.BotChannel.SendMessageAsync($"Message {i}"));
		}

		foreach (int i in Enumerable.Range(0, wtbMessages))
		{
			foreach (int r in Enumerable.Range(0, reactionsPerMessage))
			{
				var message = await BotTester.BotChannel.GetMessageAsync(messages[i].Id);
				await message.AddReactionAsync(ReactionEmotes[r].Parsed);
			}
		}

		return messages;
	}
}