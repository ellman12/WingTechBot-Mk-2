namespace ModelTests;

///Represents any test for database models.
[TestFixture]
public abstract class ModelTests
{
	[SetUp]
	public async Task SetUp()
	{
		await RecreateDatabase();
	}

	private static async Task RecreateDatabase()
	{
		await using BotDbContext context = new();
		await context.Database.EnsureDeletedAsync();
		await context.Database.EnsureCreatedAsync();
	}
}