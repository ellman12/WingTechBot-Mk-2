namespace ModelTests.ReactionTests.QueryTests;

[TestFixture]
public sealed class ReactionQueryTests : ReactionTests
{
	[TestCase]
	public async Task GetReactionsUserReceived()
	{
		int year = DateTime.Now.Year;
		await ReactionSeeder.Seed(420, 250, 400, 4, 20, 24);
		await LegacyKarma.ImportFile(Path.Combine(KarmaTestsPath, "karma.txt"), year);
		await LegacyKarma.ImportFile(Path.Combine(KarmaTestsPath, "karma.txt"), 2022); //If all works properly these will be ignored.
		await Task.Delay(Constants.ModelTestDelay);

		await using BotDbContext context = new();
		var testUser = context.Reactions.First().ReceiverId;
		var results = await Reaction.GetReactionsUserReceived(testUser, year);

		Assert.NotZero(results.Count);
		Assert.That(results.All(result => result.Value > 0));
		Assert.AreEqual(results.First(r => r.Key.Name == "upvote").Value, 3);
		Assert.AreEqual(results.First(r => r.Key.Name == "downvote").Value, 2);
		Assert.AreEqual(results.First(r => r.Key.Name == "platinum").Value, 2);
	}
}