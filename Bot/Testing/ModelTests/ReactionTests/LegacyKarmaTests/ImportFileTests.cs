namespace ModelTests.ReactionTests.LegacyKarmaTests;

public sealed class ImportFileTests : ReactionTests
{
	[TestCase("karma.txt", 2022)]
	public async Task ImportFile(string filename, int year)
	{
		string filePath = Path.Combine(KarmaTestsPath, filename);
		string[] lines = await File.ReadAllLinesAsync(filePath);

		await LegacyKarma.ImportFile(filePath, year);

		await using BotDbContext context = new();
		Assert.AreEqual(lines.Length, await context.LegacyKarma.CountAsync());
		Assert.That(await context.LegacyKarma.AllAsync(k => k.Year == year));
	}
}