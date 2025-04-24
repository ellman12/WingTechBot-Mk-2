namespace WingTechBot.Commands.VC;

public sealed class SoundboardSound
{
	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("sound_id"), JsonConverter(typeof(StringUInt64Converter))]
	public ulong SoundId { get; set; }

	// [JsonPropertyName("volume")]
	// public double Volume { get; set; }
	//
	// [JsonPropertyName("emoji_id")]
	// public ulong? EmojiId { get; set; }
	//
	// [JsonPropertyName("emoji_name")]
	// public string EmojiName { get; set; }

	[JsonPropertyName("guild_id"), JsonConverter(typeof(StringUInt64Converter))]
	public ulong? GuildId { get; set; }

	// [JsonPropertyName("available")]
	// public bool Available { get; set; }

	// [JsonPropertyName("user")]
	// public SocketGuildUser User { get; set; }
}