using System.Text.Json;

namespace WingTechBot;

public sealed record Config
{
	public string LoginToken { get; init; }
	
	public ulong UserId { get; init; }

	public ulong ServerId { get; init; }

	public ulong BotChannelId { get; init; }

	public ulong ModRoleId { get; init; }

	///Usernames of people who can run bot admin commands.
	public string[] BotAdmins { get; init; }

	///Any attempt to give karma before this date is ignored.
	public DateTime StartDate { get; init; }

	public string StatusMessage { get; init; }

	///Read in config.json from project root and parse it.
	public static Config FromJson()
	{
		#if DEBUG
		string path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName, "config.json");
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