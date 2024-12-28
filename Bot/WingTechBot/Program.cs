namespace WingTechBot;

public static class Program
{
	public static WingTechBot Bot { get; private set; }
	
	public static void Main()
	{
		using BotDbContext context = new();
		context.Database.EnsureCreated();

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