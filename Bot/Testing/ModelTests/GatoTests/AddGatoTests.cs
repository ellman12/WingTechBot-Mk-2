namespace ModelTests.GatoTests;

public sealed class AddGatoTests : ModelTests
{
	private static readonly TestCaseData[] ValidGatos =
	[
		new("Stormy", 123456ul, "https://cdn.discordapp.com/attachments/this_isnt_a_real_image.jpg"),
		new("A Cat Name With Spaces", 69420ul, "https://cdn.discordapp.com/attachments/thisisanotherfile.png"),
		new(null, 3ul, "https://cdn.discordapp.com/attachments/thisisanotherfile.png")
	];

	private static readonly TestCaseData[] InvalidGatos =
	[
		new("invalid gato 1", 123456ul, "kjafhjkasdhf"),
		new("invalid gato 2", 0ul, "https://cdn.discordapp.com/attachments/thisisafile.png")
	];

	[TestCaseSource(nameof(ValidGatos))]
	public async Task AddGato(string name, ulong uploaderId, string url)
	{
		await using BotDbContext context = new();
		Assert.False(await context.Gatos.AnyAsync());

		await Gato.AddGato(url, name, uploaderId);
		await Task.Delay(Constants.DatabaseDelay);

		Assert.AreEqual(await context.Gatos.CountAsync(), 1);
		Assert.AreEqual((await Gato.Find(url, name, uploaderId)).Name, name);
	}

	[TestCaseSource(nameof(ValidGatos))]
	public async Task FailsWhenGatoExists(string name, ulong uploaderId, string url)
	{
		await AddGato(name, uploaderId, url);
		Assert.ThrowsAsync<ArgumentException>(async () => await Gato.AddGato(url, name, uploaderId));
	}

	[TestCaseSource(nameof(InvalidGatos))]
	public void FailsForInvalidGatos(string name, ulong uploaderId, string url)
	{
		Assert.ThrowsAsync<ArgumentException>(async () => await Gato.AddGato(url, name, uploaderId));
	}
}