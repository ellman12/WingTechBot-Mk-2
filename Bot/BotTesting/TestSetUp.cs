namespace BotTesting;

///Global set up before any tests are run.
[SetUpFixture]
public sealed class TestSetUp
{
	[OneTimeSetUp]
	public void GlobalSetup()
	{
		EnvHelper.ReadEnvVariables();
	}

	[OneTimeTearDown]
	public void GlobalTeardown() {}
}