using Discord;
using Discord.WebSocket;
using WingTechBot.Commands;

namespace WingTechBot;

public sealed class WingTechBot
{
	public DiscordSocketClient Client { get; } = new(DiscordConfig);

	public Config Config { get; } = Config.FromJson();

	public SocketTextChannel BotChannel { get; private set; }

	public SlashCommandHandler CommandHandler { get; private set; }

	public static readonly DiscordSocketConfig DiscordConfig = new() {MessageCacheSize = 100, AlwaysDownloadUsers = true};

	private WingTechBot() {}

	public static async Task<WingTechBot> Create()
	{
		WingTechBot bot = new();
		bot.Client.Log += Logger.LogLine;
		bot.Client.Ready += bot.OnClientReady;

		await bot.Client.LoginAsync(TokenType.Bot, bot.Config.LoginToken);
		await bot.Client.SetCustomStatusAsync(bot.Config.StatusMessage);
		await bot.Client.StartAsync();
		return bot;
	}

	private async Task OnClientReady()
	{
		BotChannel = Client.GetChannel(Config.BotChannelID) as SocketTextChannel;

		if (BotChannel == null)
			throw new NullReferenceException("Could not find bot channel");

		CommandHandler = await SlashCommandHandler.Create(this);
		Client.SlashCommandExecuted += CommandHandler.SlashCommandExecuted;

		await BotChannel.SendMessageAsync("Bot started and ready");
	}
}