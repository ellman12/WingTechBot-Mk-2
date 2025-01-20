namespace IntegrationTests;
using WingTechBot=WingTechBot.WingTechBot;

///Represents any kind of integration test.
[TestFixture]
public abstract class IntegrationTest
{
	public static WingTechBot WingTechBot { get; internal set; }
	public static WingTechBotTester BotTester { get; internal set; }
	
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