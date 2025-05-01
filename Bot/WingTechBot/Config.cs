namespace WingTechBot;

public sealed record Config
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public LogSeverity LogLevel { get; init; }

	public string LoginToken { get; init; }
	
	public ulong UserId { get; init; }

	public ulong ServerId { get; init; }

	public ulong BotChannelId { get; init; }

	public string ServerUrl { get; init; }

	///The IDs of Discord servers the bot can get sounds from.
	public ulong[] SoundboardServerIds { get; init; }

	///Default VC to join when /join-vc invoked and no other channel given.
	public ulong DefaultVoiceChannelId { get; init; }

	///The ID of the role for the bot.
	public ulong BotRoleId { get; init; }

	public ulong ModRoleId { get; init; }

	///IDs of users who can run bot admin commands.
	public ulong[] BotAdmins { get; init; }

	///Any attempt to give karma before this date is ignored.
	public DateTime StartDate { get; init; }

	public string StatusMessage { get; init; }

	///Used to interface with an LLM API.
	public string LLMToken { get; init; }

	///Used to tell the LLM how to behave.
	public string LLMBehavior { get; init; }

	///Sound events for <see cref="Commands.VC.AutoSounds"/>. Event name > user IDs > sound IDs.
	public IReadOnlyDictionary<string, IReadOnlyDictionary<ulong, ulong[]>> AutoSounds { get; init; }

	///Read in config.json from project root and parse it.
	public static Config FromJson()
	{
		#if DEBUG
		string path = Path.Combine(Program.ProjectRoot, "config.json");
		#else
		string path = "/app/config.json";
		#endif
		
		return FromJson(path);
	}

	public static Config FromJson(string path)
	{
		return String.IsNullOrWhiteSpace(path) ? FromJson() : JsonSerializer.Deserialize<Config>(File.ReadAllText(path));
	}
}