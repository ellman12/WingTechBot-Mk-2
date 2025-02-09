using WingTechBot.Commands.Gatos;
using WingTechBot.Commands.Reactions;

namespace WingTechBot;

public sealed class WingTechBot
{
	public DiscordSocketClient Client { get; } = new(DiscordConfig);

	public Config Config { get; private set; }

	public SocketGuild Guild { get; private set; }

	public SocketTextChannel BotChannel { get; private set; }

	private static readonly DiscordSocketConfig DiscordConfig = new()
	{
		MessageCacheSize = 100,
		AlwaysDownloadUsers = true,
		GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
	};

	private WingTechBot() {}

	private readonly ReactionTracker reactionTracker = new();
	private readonly ReactionsCommand reactionsCommand = new();
	private readonly ReactionsFromCommand reactionsFromCommand = new();
	private readonly ReactionsGivenCommand reactionsGivenCommand = new();
	private readonly TopCommand topCommand = new();
	private readonly TopEmotesCommand topEmotesCommand = new();
	private readonly TopMessagesCommand topMessagesCommand = new();

	private readonly InfoCommand infoCommand = new();

	private readonly GatoCommand gatoCommand = new();
	private readonly GatoAddCommand gatoAddCommand = new();
	private readonly TopGatosCommand topGatosCommand = new();

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

		await SetUpCommands();

		#if RELEASE
		await BotChannel.SendMessageAsync("Bot started and ready");
		#endif
	}

	private async Task SetUpCommands()
	{
		if (SlashCommand.NoRecreateCommands)
		{
			Logger.LogLine("Commands will not be recreated");
		}
		else
		{
			Logger.LogLine("Clearing slash commands");
			await ClearCommands();
			Logger.LogLine("Setting up slash commands");
		}

		Client.SlashCommandExecuted += PreprocessCommand;

		await reactionsCommand.SetUp(this);
		await reactionsFromCommand.SetUp(this);
		await reactionsGivenCommand.SetUp(this);
		await topCommand.SetUp(this);
		await topEmotesCommand.SetUp(this);
		await topMessagesCommand.SetUp(this);
		await infoCommand.SetUp(this);
		await gatoCommand.SetUp(this);
		await gatoAddCommand.SetUp(this);
		await topGatosCommand.SetUp(this);
	}

	///Removes all slash commands from the bot. However, because Discord is terrible this is unreliable and often does nothing.
	private async Task ClearCommands()
	{
		var guild = Client.GetGuild(Config.ServerId);
		await guild.DeleteApplicationCommandsAsync();
	}

	private async Task PreprocessCommand(SocketSlashCommand command)
	{
		//Makes command timeout longer than 3 seconds. Essential for debug breakpoints.
		await command.DeferAsync();
	}
}
