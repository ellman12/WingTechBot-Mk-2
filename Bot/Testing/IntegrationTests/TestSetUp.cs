namespace IntegrationTests;
using WingTechBot=WingTechBot.WingTechBot;

///Global set up before any tests are run.
[SetUpFixture]
public sealed class TestSetUp
{
	[OneTimeSetUp]
	public static async Task OneTimeSetUp()
	{
		EnvHelper.ReadEnvVariables();

		string projectRoot = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;

		IntegrationTest.WingTechBot = await WingTechBot.Create(Path.Combine(projectRoot, "wtb_config.json"));
		IntegrationTest.BotTester = await WingTechBotTester.Create(Path.Combine(projectRoot, "bt_config.json"));

		//Wait for these to initialize.
		while (IntegrationTest.WingTechBot.BotChannel == null)
			await Task.Delay(100);

		while (IntegrationTest.BotTester.BotChannel == null)
			await Task.Delay(100);

		await IntegrationTest.BotTester.BotChannel.SendMessageAsync("Begin integration tests");
	}

	[OneTimeTearDown]
	public static async Task OneTimeTearDown()
	{
		await IntegrationTest.BotTester.BotChannel.SendMessageAsync("Finish integration tests");
	}
}