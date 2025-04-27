namespace WingTechBot;

public sealed class WingTechBot
{
	public DiscordSocketClient Client { get; } = new(DiscordConfig);

	public Config Config { get; private init; }

	public SocketGuild Guild { get; private set; }

	public SocketTextChannel BotChannel { get; private set; }

	public SocketVoiceChannel DefaultVoiceChannel { get; private set; }

	public Communication Communication { get; private set; }

	public HttpClient HttpClient { get; } = new();

	private static readonly DiscordSocketConfig DiscordConfig = new()
	{
		MessageCacheSize = 100,
		AlwaysDownloadUsers = true,
		GatewayIntents = (GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent | GatewayIntents.GuildMembers) & ~(GatewayIntents.GuildScheduledEvents | GatewayIntents.GuildInvites)
	};

	private WingTechBot() {}

	private readonly ConcurrentDictionary<string, SlashCommand> slashCommands = new();
	private readonly ReactionTracker reactionTracker = new();
	public VoiceChannelConnection VoiceChannelConnection { get; } = new();

	public GameHandler GameHandler { get; private set; }

	public static async Task<WingTechBot> Create(Config config)
	{
		WingTechBot bot = new() {Config = config};

		bot.Client.Log += Logger.LogLine;
		bot.Client.Ready += bot.OnClientReady;

		await bot.Client.LoginAsync(TokenType.Bot, bot.Config.LoginToken);
		await bot.Client.SetCustomStatusAsync(bot.Config.StatusMessage);
		await bot.Client.StartAsync();

		bot.reactionTracker.SetUp(bot);

		bot.GameHandler = new GameHandler(bot);

		return bot;
	}

	public static async Task<WingTechBot> Create(string configPath = null)
	{
		var config = String.IsNullOrWhiteSpace(configPath) ? Config.FromJson() : Config.FromJson(configPath);
		return await Create(config);
	}

	private async Task OnClientReady()
	{
		Guild = Client.GetGuild(Config.ServerId) ?? throw new NullReferenceException("Could not find guild");
		BotChannel = Client.GetChannel(Config.BotChannelId) as SocketTextChannel ?? throw new NullReferenceException("Could not find bot channel");
		DefaultVoiceChannel = Client.GetChannel(Config.DefaultVoiceChannelId) as SocketVoiceChannel ?? throw new NullReferenceException("Could not find voice channel");
		Communication = new Communication(this);

		await VoiceChannelConnection.SetUp(this);

		await SetUpCommands();

		#if RELEASE
		await BotChannel.SendMessageAsync("Bot started and ready");
		#endif
	}

	private async Task SetUpCommands()
	{
		Client.SlashCommandExecuted += HandleCommand;

		if (SlashCommand.NoRecreateCommands)
		{
			Logger.LogLine("Commands will not be recreated");
		}
		else
		{
			Logger.LogLine("Clearing slash commands");
			await Client.BulkOverwriteGlobalApplicationCommandsAsync(Enumerable.Empty<ApplicationCommandProperties>().ToArray());
			Logger.LogLine("Setting up slash commands");
		}

		var commands = typeof(WingTechBot).Assembly
			.GetTypes()
			.Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(SlashCommand)))
			.Select(Activator.CreateInstance)
			.Cast<SlashCommand>();

		// await Parallel.ForEachAsync(commands, async (command, _) =>
		foreach (var command in commands)
		{
			await command.SetUp(this);

			if (!slashCommands.TryAdd(command.Name, command))
				await Logger.LogExceptionAsMessage(new Exception($"Error initializing {command.Name} command"), BotChannel);
		}
		// });
	}

	private async Task HandleCommand(SocketSlashCommand command)
	{
		string name = command.CommandName;

		try
		{
			if (slashCommands.TryGetValue(name, out var slashCommand))
			{
				if (slashCommand.Admin && !Program.Config.BotAdmins.Contains(command.User.Id))
				{
					await command.FollowupAsync("You are not authorized to use this command");
					return;
				}

				if (slashCommand.Defer)
					await command.DeferAsync();

				await slashCommand.HandleCommand(command);
			}
			else
				await Logger.LogExceptionAsMessage(new Exception($"Command {name} not found"), command.Channel);
		}
		catch (Exception e)
		{
			await Logger.LogExceptionAsMessage(e, command.Channel);
		}
	}
}