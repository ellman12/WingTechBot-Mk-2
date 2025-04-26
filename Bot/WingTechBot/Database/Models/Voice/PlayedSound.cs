namespace WingTechBot.Database.Models.Voice;

///Represents an instance of a <see cref="SoundboardSound"/> played in a VC.
public sealed partial class PlayedSound(ulong userId, ulong channelId, ulong soundId) : Model
{
	[Key]
	public int Id { get; private init; }

	[Required]
	public ulong UserId { get; private init; } = userId;

	[Required]
	public ulong ChannelId { get; private init; } = channelId;

	[Required]
	public ulong SoundId { get; private init; } = soundId;

	[Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public DateTime CreatedAt { get; private init; }

	public SoundboardSound Sound { get; private init; }
}

public sealed class PlayedSoundConfiguration : IEntityTypeConfiguration<PlayedSound>
{
	public void Configure(EntityTypeBuilder<PlayedSound> builder)
	{
		builder.Property(e => e.Id).ValueGeneratedOnAdd();
		builder.Property(e => e.CreatedAt).HasDefaultValueSql("timezone('utc', now())");

		builder.HasOne(p => p.Sound)
			.WithMany(s => s.PlayedSounds)
			.HasForeignKey(p => p.SoundId)
			.OnDelete(DeleteBehavior.NoAction);
	}
}