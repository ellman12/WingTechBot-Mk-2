namespace WingTechBot;

public static class Program
{
	public static WingTechBot Bot { get; private set; }

	public static readonly string ProjectRoot = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;

	public static void Main()
	{
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
