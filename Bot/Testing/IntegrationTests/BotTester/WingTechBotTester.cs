namespace IntegrationTests.BotTester;

///Used for WingTech Bot Mk 2 integration testing. Simulates a real human user by sending reactions to the bot's messages, among other things.
public sealed class WingTechBotTester
{
	public DiscordSocketClient Client { get; } = new(DiscordConfig);

	public Config Config { get; private set; }

	public SocketTextChannel BotChannel { get; private set; }

	public static readonly DiscordSocketConfig DiscordConfig = new() {MessageCacheSize = 100, AlwaysDownloadUsers = true};

	private WingTechBotTester() {}

	public static async Task<WingTechBotTester> Create(string configPath = null)
	{
		WingTechBotTester bot = new();
		bot.Config = String.IsNullOrWhiteSpace(configPath) ? Config.FromJson() : Config.FromJson(configPath);

		bot.Client.Log += Logger.LogLine;
		bot.Client.Ready += bot.OnClientReady;

		await bot.Client.LoginAsync(TokenType.Bot, bot.Config.LoginToken);
		await bot.Client.SetCustomStatusAsync(bot.Config.StatusMessage);
		await bot.Client.StartAsync();

		return bot;
	}

	private async Task OnClientReady()
	{
		BotChannel = Client.GetChannel(Config.BotChannelId) as SocketTextChannel;

		if (BotChannel == null)
			throw new NullReferenceException("Could not find bot channel");
	}
}
