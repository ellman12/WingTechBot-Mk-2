namespace WingTechBot.Database.Models.Voice;

///Represents a sound played through either Discord's Soundboard feature or a <see cref="SocketVoiceChannel"/>.
public sealed partial class SoundboardSound(ulong id, string name, byte[] audio) : Model
{
	///This is not auto-incrementing because EF Core does not support bigserial
	[Key, JsonPropertyName("sound_id"), JsonConverter(typeof(StringUInt64Converter))]
	public ulong Id { get; init; } = id;

	[Required, JsonPropertyName("name")]
	public string Name { get; init; } = name;

	[NotMapped, JsonPropertyName("guild_id"), JsonConverter(typeof(StringUInt64Converter))]
	public ulong? GuildId { get; init; }

	///What is sent through FFmpeg to be heard in the <see cref="SocketVoiceChannel"/>.
	[JsonPropertyName("audio"), JsonConverter(typeof(ByteArrayBase64Converter))]
	public byte[] Audio { get; set; } = audio;

	[Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public DateTime CreatedAt { get; init; }

	[NotMapped, JsonPropertyName("type")]
	public string Type => GuildId == null && Audio != null ? "voice" : "soundboard";
}

public sealed class SoundboardSoundConfiguration : IEntityTypeConfiguration<SoundboardSound>
{
	public void Configure(EntityTypeBuilder<SoundboardSound> builder)
	{
		builder.Property(e => e.Audio).IsRequired();
		builder.Property(e => e.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
	}
}