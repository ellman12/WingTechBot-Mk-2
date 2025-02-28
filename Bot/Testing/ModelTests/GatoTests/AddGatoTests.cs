namespace ModelTests.GatoTests;

public sealed class AddGatoTests : ModelTests
{
	private static readonly TestCaseData[] ValidGatos =
	[
		new("stormy", "filename1.png", 123456ul, "https://en.wikipedia.org/wiki/Black_cat#/media/File:Blackcat-Lilith.jpg"),
		new("a cat name with spaces", "name of the file.jpg", 69420ul, "https://tenor.com/view/mwah-cat-mwah-cat-kissing-black-cat-kissing-black-cat-gif-11587383618549829093"),
		new("A Name of a Cat", "nameforthisfile.mp4", 3ul, "https://en.wikipedia.org/wiki/Black_cat#/media/File:Gladstone_a_year_in_Treasury_(1).jpg")
	];

	private static readonly TestCaseData[] InvalidGatos =
	[
		new("invalid gato 2", "filename.png", 0ul, "https://tenor.com/view/mwah-cat-mwah-cat-kissing-black-cat-kissing-black-cat-gif-11587383618549829093"),
		new(null, "filename with space.mov", 69420ul, "https://en.wikipedia.org/wiki/Black_cat#/media/File:Gladstone_a_year_in_Treasury_(1).jpg"),
		new("A Name of a Cat", null, 3ul, "https://en.wikipedia.org/wiki/Black_cat#/media/File:Gladstone_a_year_in_Treasury_(1).jpg")
	];

	[TestCaseSource(nameof(ValidGatos))]
	public async Task AddGato(string name, string filename, ulong uploaderId, string url)
	{
		await using BotDbContext context = new();
		Assert.IsEmpty(context.Gatos);

		using HttpClient client = new();
		var response = await client.GetAsync(url);
		var media = await response.Content.ReadAsByteArrayAsync();

		await Gato.AddGato(media, filename, name, uploaderId);
		await Task.Delay(Constants.ModelTestDelay);

		Assert.AreEqual(1, await context.Gatos.CountAsync());
	}

	[TestCaseSource(nameof(InvalidGatos))]
	public async Task FailsForInvalidGatos(string name, string filename, ulong uploaderId, string url)
	{
		using HttpClient client = new();
		var response = await client.GetAsync(url);
		var media = await response.Content.ReadAsByteArrayAsync();

		Assert.ThrowsAsync<ArgumentException>(async () => await Gato.AddGato(media, filename, name, uploaderId));
	}
}