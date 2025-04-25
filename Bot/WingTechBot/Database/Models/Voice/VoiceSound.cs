namespace WingTechBot.Database.Models.Voice;

///Represents any kind of audio clip &gt; 5 seconds played through voice.
public sealed partial class VoiceSound(string name, byte[] audio) : Model
{
	[Key]
	public ulong Id { get; private init; }

	[Required]
	public string Name { get; private init; } = name;

	///What is sent through FFmpeg to be heard in the <see cref="SocketVoiceChannel"/>.
	[Required]
	public byte[] Audio { get; private init; } = audio;

	[Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public DateTime CreatedAt { get; private init; }
}

public sealed class VoiceSoundConfiguration : IEntityTypeConfiguration<VoiceSound>
{
	public void Configure(EntityTypeBuilder<VoiceSound> builder)
	{
		builder.Property(e => e.Id).ValueGeneratedOnAdd();
		builder.Property(e => e.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
	}
}