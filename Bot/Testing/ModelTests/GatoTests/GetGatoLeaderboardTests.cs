namespace ModelTests.GatoTests;

public sealed class GetGatoLeaderboardTests : ModelTests
{
	private static readonly Gato[] ValidGatos =
	[
		new("https://cdn.discordapp.com/attachments/this_isnt_a_real_image.jpg", "stormy", 123456ul),
		new("https://cdn.discordapp.com/attachments/thisisanotherfile.png", "a cat name with spaces", 69420ul),
		new("https://cdn.discordapp.com/attachments/thisisanotherfile.png", null, 3ul)
	];

	[TestCase]
	public async Task GetGatoLeaderboard()
	{
		await using BotDbContext context = new();
		Assert.False(await context.Gatos.AnyAsync());

		foreach (var gato in ValidGatos)
			await Gato.AddGato(gato.Url, gato.Name, gato.UploaderId);

		await Task.Delay(Constants.DatabaseDelay);

		Assert.AreEqual(await context.Gatos.CountAsync(), ValidGatos.Length);
		Assert.That(await context.Gatos.AsAsyncEnumerable().AllAsync(g => ValidGatos.FirstOrDefault(v => v.Name == g.Name) != null));
	}
}