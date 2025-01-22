using Discord;

namespace IntegrationTests.ReactionTracker;

public abstract class ReactionTrackerTests : IntegrationTests
{
	public static readonly ReactionEmote[] ReactionEmotes =
	[
		new("upvote", 1325537736626671697ul),
		new("downvote", 1328082903703224330ul),
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
}