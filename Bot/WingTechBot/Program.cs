namespace WingTechBot;

public static class Program
{
	#if DEBUG
	public static readonly string ProjectRoot = Environment.CurrentDirectory;
	#elif RELEASE
	public const string ProjectRoot = "/app";
	#endif

	public static WingTechBot Bot { get; private set; }

	public static Config Config { get; } = Config.FromJson();

	public static void Main(string[] args)
	{
		if (args.Any(arg => arg == "--no-recreate-commands"))
		{
			SlashCommand.NoRecreateCommands = true;
		}

		try
		{
			using BotDbContext context = new();
			context.RunMigrationsIfNeeded();
		}
		catch (Exception e)
		{
			Console.WriteLine("Error starting database");
			Console.WriteLine(e.Message);
		}

		try
		{
			MainAsync().GetAwaiter().GetResult();
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			Console.ReadLine();
			throw;
		}
	}

	private static async Task MainAsync()
	{
		await WingSounds.Create(Config);
		Bot = await WingTechBot.Create(Config);

		await Task.Delay(Timeout.Infinite);
	}
}