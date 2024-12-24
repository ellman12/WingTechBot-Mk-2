using System;
using System.IO;
using System.Text.Json;

namespace WingTechBot;

public sealed record Config
{
	#if DEBUG
	public const string ConfigPath = "C:/config.json";
	#else
	public const string ConfigPath = "/app/config.json";
	#endif
	
	public string LoginToken { get; init; }

	public ulong BotChannelID { get; init; }

	public ulong ModRoleID { get; init; }
	
	///Usernames of people who can run bot admin commands.
	public string[] BotAdmins { get; init; }
	
	///Any attempt to give karma before this date is ignored.
	public DateOnly StartDate { get; init; }
	
	public string StatusMessage { get; init; }

	///Read in config.json and parse it.
	public static Config FromJson() => JsonSerializer.Deserialize<Config>(File.ReadAllText(ConfigPath));
}
