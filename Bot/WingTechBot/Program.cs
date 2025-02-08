namespace WingTechBot;

public static class Program
{
	public static WingTechBot Bot { get; private set; }

	#if DEBUG
	public static readonly string ProjectRoot = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
	#elif RELEASE
	public const string ProjectRoot = "/app";
	#endif

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
		Bot = await WingTechBot.Create();
		await Task.Delay(Timeout.Infinite);
	}
}