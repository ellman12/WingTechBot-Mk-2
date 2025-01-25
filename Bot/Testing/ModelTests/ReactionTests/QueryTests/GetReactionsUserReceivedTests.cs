namespace ModelTests.ReactionTests.QueryTests;

public sealed class GetReactionsUserReceivedTests : ReactionTests
{
	[TestCase]
	public async Task GetReactionsUserReceived()
	{
		await ReactionSeeder.Seed(420, 250, 400, 4, 20, 24);

		await using BotDbContext context = new();
		var testUser = context.Reactions.First();
		var results = await Reaction.GetReactionsUserReceived(testUser.ReceiverId);

		Assert.That(results.Length > 0);
		Assert.That(results.All(result => result.count > 0));
	}
}