namespace WingTechBot.Database.Models.Voice;

///Represents a sound played through either Discord's Soundboard feature or a <see cref="SocketVoiceChannel"/>.
public sealed partial class SoundboardSound(string name, byte[] audio) : Model
{
	[Key, JsonPropertyName("sound_id"), JsonConverter(typeof(StringUInt64Converter))]
	public ulong Id { get; init; }

	[Required, JsonPropertyName("name")]
	public string Name { get; init; } = name;

	[NotMapped, JsonPropertyName("guild_id"), JsonConverter(typeof(StringUInt64Converter))]
	public ulong? GuildId { get; init; }

	///What is sent through FFmpeg to be heard in the <see cref="SocketVoiceChannel"/>.
	[JsonConverter(typeof(ByteArrayBase64Converter))]
	public byte[] Audio { get; init; } = audio;

	[Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public DateTime CreatedAt { get; init; }
}

public sealed class SoundboardSoundConfiguration : IEntityTypeConfiguration<SoundboardSound>
{
	public void Configure(EntityTypeBuilder<SoundboardSound> builder)
	{
		builder.Property(e => e.Id).ValueGeneratedOnAdd();
		builder.Property(e => e.Audio).IsRequired();
		builder.Property(e => e.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
	}
}