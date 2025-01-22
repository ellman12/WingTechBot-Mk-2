using WingTechBot.Commands.Reactions;

namespace WingTechBot;

public sealed class WingTechBot
{
	public DiscordSocketClient Client { get; } = new(DiscordConfig);

	public Config Config { get; private set; }

	public SocketGuild Guild { get; private set; }

	public SocketTextChannel BotChannel { get; private set; }

	public static readonly DiscordSocketConfig DiscordConfig = new() {MessageCacheSize = 100, AlwaysDownloadUsers = true};

	private WingTechBot() {}

	private readonly ReactionTracker reactionTracker = new();

	public static async Task<WingTechBot> Create(string configPath = null)
	{
		WingTechBot bot = new();
		bot.Config = String.IsNullOrWhiteSpace(configPath) ? Config.FromJson() : Config.FromJson(configPath);

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
		Guild = Client.GetGuild(Config.ServerId) ?? throw new NullReferenceException("Could not find guild");
		BotChannel = Client.GetChannel(Config.BotChannelId) as SocketTextChannel ?? throw new NullReferenceException("Could not find bot channel");

		// await BotChannel.SendMessageAsync("Bot started and ready");
	}
}