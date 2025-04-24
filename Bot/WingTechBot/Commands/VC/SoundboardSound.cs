namespace WingTechBot.Commands.VC;

///Represents a sound &lt;= 5 seconds played through Discord's Soundboard feature.
public sealed class SoundboardSound
{
	[JsonPropertyName("name")]
	public string Name { get; private init; }

	[JsonPropertyName("sound_id"), JsonConverter(typeof(StringUInt64Converter))]
	public ulong SoundId { get; private init; }

	[JsonPropertyName("guild_id"), JsonConverter(typeof(StringUInt64Converter))]
	public ulong? GuildId { get; private init; }
}