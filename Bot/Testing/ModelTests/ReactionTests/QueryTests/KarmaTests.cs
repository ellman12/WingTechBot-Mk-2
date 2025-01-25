namespace ModelTests.ReactionTests.QueryTests;

[TestFixture]
public sealed class KarmaTests : ReactionTests
{
	[TestCase]
	public async Task GetKarmaLeaderboard()
	{
		int year = DateTime.Now.Year;
		await ReactionSeeder.Seed(420, 250, 400, 4, 20, 24);

		await using BotDbContext context = new();
		var results = await Karma.GetKarmaLeaderboard(year);

		Assert.That(results.Length == 0);

		var upvote = await context.ReactionEmotes.FirstAsync(e => e.Name == "upvote");
		await upvote.SetKarmaValue(1);
		var downvote = await context.ReactionEmotes.FirstAsync(e => e.Name == "downvote");
		await upvote.SetKarmaValue(-1);

		results = await Karma.GetKarmaLeaderboard(year);
		Assert.That(results.Length > 0);
		Assert.That(results.Length == results.Select(r => r.receiverId).Distinct().Count());

		//Ensure past years are ignored.
		foreach (int i in Enumerable.Range(1, 5))
		{
			ulong messageId = 123456 * (ulong)i;
			await Reaction.AddReaction(69, 420, messageId, "upvote", upvote.DiscordEmoteId);
			await Reaction.AddReaction(69, 420, messageId, "downvote", downvote.DiscordEmoteId);

			string interval = $"{i} YEAR{(i > 1 ? "S" : "")}";
			await context.Database.ExecuteSqlRawAsync($"UPDATE \"Reactions\" SET \"CreatedAt\" = \"CreatedAt\" - INTERVAL '{interval}' WHERE \"MessageId\" = {messageId}");
		}
		
		var newResults = await Karma.GetKarmaLeaderboard(year);
		Assert.That(results.SequenceEqual(newResults));
	}
}