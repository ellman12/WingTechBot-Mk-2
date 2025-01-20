using WingTechBot.Commands.Reactions;

namespace WingTechBot;

public sealed class WingTechBot
{
	public DiscordSocketClient Client { get; } = new(DiscordConfig);

	public Config Config { get; } = Config.FromJson();

	public SocketTextChannel BotChannel { get; private set; }

	public static readonly DiscordSocketConfig DiscordConfig = new() {MessageCacheSize = 100, AlwaysDownloadUsers = true};

	private WingTechBot() {}

	private readonly ReactionTracker reactionTracker = new();

	public static async Task<WingTechBot> Create()
	{
		WingTechBot bot = new();
		bot.Client.Log += Logger.LogLine;
		bot.Client.Ready += bot.OnClientReady;

		await bot.Client.LoginAsync(TokenType.Bot, bot.Config.LoginToken);
		await bot.Client.SetCustomStatusAsync(bot.Config.StatusMessage);
		await bot.Client.StartAsync();

		bot.reactionTracker.SetUp(bot);

		return bot;
	}

	private async Task OnClientReady()
	{
		BotChannel = Client.GetChannel(Config.BotChannelId) as SocketTextChannel;

		if (BotChannel == null)
			throw new NullReferenceException("Could not find bot channel");

		await BotChannel.SendMessageAsync("Bot started and ready");
	}
}
