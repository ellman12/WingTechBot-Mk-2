namespace ModelTests.GatoTests;

public sealed class GetGatoLeaderboardTests : ModelTests
{
	private static readonly (string url, string filename, string name, ulong uploaderId)[] ValidGatos =
	[
		new("https://cdn.discordapp.com/attachments/this_isnt_a_real_image.jpg", "filename1.jpg", "stormy", 123456ul),
		new("https://cdn.discordapp.com/attachments/thisisanotherfile.png", "anotherfile.mp4", "a cat name with spaces", 69420ul),
		new("https://cdn.discordapp.com/attachments/thisisanotherfile.png", "idk lol.png", "cat1", 3ul)
	];

	[TestCase]
	public async Task GetGatoLeaderboard()
	{
		await using BotDbContext context = new();
		Assert.False(await context.Gatos.AnyAsync());

		foreach (var gato in ValidGatos)
		{
			using HttpClient client = new();
			var response = await client.GetAsync(gato.url);
			var media = await response.Content.ReadAsByteArrayAsync();

			await Gato.AddGato(media, gato.filename, gato.name, gato.uploaderId);
		}

		await Task.Delay(Constants.ModelTestDelay);

		Assert.AreEqual(await context.Gatos.CountAsync(), ValidGatos.Length);
		Assert.That(await context.Gatos.AsAsyncEnumerable().AllAsync(g => ValidGatos.Any(v => v.name == g.Name)));
	}
}