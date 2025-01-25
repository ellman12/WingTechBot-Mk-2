namespace ModelTests.ReactionTests.QueryTests;

[TestFixture]
public sealed class KarmaTests : ReactionTests
{
	[TestCase]
	public async Task GetKarmaLeaderboard()
	{
		await ReactionSeeder.Seed(420, 250, 400, 4, 20, 24);

		await using BotDbContext context = new();
		var results = await Karma.GetKarmaLeaderboard(DateTime.Now.Year);
		
		Assert.That(results.Length == 0);

		await context.ReactionEmotes.First(e => e.Name == "upvote").SetKarmaValue(1);
		await context.ReactionEmotes.First(e => e.Name == "downvote").SetKarmaValue(-1);

		results = await Karma.GetKarmaLeaderboard(DateTime.Now.Year);
		Assert.That(results.Length > 0);
	}
}