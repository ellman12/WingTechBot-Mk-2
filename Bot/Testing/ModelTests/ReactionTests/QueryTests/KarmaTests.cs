namespace ModelTests.ReactionTests.QueryTests;

[TestFixture]
public sealed class KarmaTests : ReactionTests
{
	[TestCase]
	public async Task GetKarmaLeaderboard()
	{
		int year = DateTime.Now.Year;
		await ReactionSeeder.Seed(420, 250, 400, 4, 20, 24);
		await LegacyKarma.ImportFile(Path.Combine(KarmaTestsPath, "karma.txt"), year);
		await LegacyKarma.ImportFile(Path.Combine(KarmaTestsPath, "karma.txt"), 2022); //If all works properly these will be ignored.
		await Task.Delay(Constants.ModelTestDelay);

		await using BotDbContext context = new();
		var upvote = await context.ReactionEmotes.FirstAsync(e => e.Name == "upvote");
		var downvote = await context.ReactionEmotes.FirstAsync(e => e.Name == "downvote");
		await upvote.SetKarmaValue(1);
		await downvote.SetKarmaValue(-1);
		await Task.Delay(Constants.ModelTestDelay);

		var results = await Karma.GetKarmaLeaderboard(year);
		Assert.IsNotEmpty(results);
		Assert.AreEqual(results.Length, results.Select(r => r.receiverId).Distinct().Count());
		Assert.AreEqual(context.LegacyKarma.Count(lk => lk.Year == year), 36);
		Assert.AreEqual(results.First(r => r.receiverId == 76).karma, 37);

		//Add additional reactions with previous years and ensure they are ignored.
		foreach (int i in Enumerable.Range(1, 5))
		{
			ulong messageId = 123456 * (ulong)i;
			await Reaction.AddReaction(69, 420, 867, messageId, "upvote", upvote.DiscordEmoteId);
			await Reaction.AddReaction(69, 420, 867, messageId, "downvote", downvote.DiscordEmoteId);

			string interval = $"{i} YEAR{(i > 1 ? "S" : "")}";
			await context.Database.ExecuteSqlRawAsync($"UPDATE \"Reactions\" SET \"CreatedAt\" = \"CreatedAt\" - INTERVAL '{interval}' WHERE \"MessageId\" = {messageId}");
		}

		var newResults = await Karma.GetKarmaLeaderboard(year);
		Assert.That(results.SequenceEqual(newResults));
		Assert.AreEqual(newResults.First(r => r.receiverId == 76).karma, 37);
	}
}