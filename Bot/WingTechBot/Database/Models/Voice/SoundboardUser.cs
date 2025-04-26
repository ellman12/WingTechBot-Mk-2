namespace WingTechBot.Database.Models.Voice;

///Represents a user who's authenticated to use the soundboard website.
public sealed partial class SoundboardUser(ulong id, ulong authenticatorId) : Model
{
	[Key]
	public ulong Id { get; private init; } = id;

	///ID of the user who gave this user auth.
	[Required]
	public ulong AuthenticatorId { get; private init; } = authenticatorId;

	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public DateTime CreatedAt { get; private init; }
}

public sealed class SoundboardUserConfiguration : IEntityTypeConfiguration<SoundboardUser>
{
	public void Configure(EntityTypeBuilder<SoundboardUser> builder)
	{
		builder.Property(e => e.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
	}
}