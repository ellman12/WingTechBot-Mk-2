namespace WingTechBot.Database.Models.Reactions;

using static DatabaseGeneratedOption;

///Represents the karma for a user during a specific year. This is the format the previous version of <see cref="WingTechBot"/> used, which didn't track individual reactions to messages.
public sealed partial class LegacyKarma(ulong userId, int year, int upvotes, int downvotes, int silver, int gold, int platinum) : Model
{
	public ulong UserId { get; private init; } = userId;

	public int Year { get; private init; } = year;

	[DatabaseGenerated(Identity)]
	public int Upvotes { get; private init; } = upvotes;

	[DatabaseGenerated(Identity)]
	public int Downvotes { get; private init; } = downvotes;

	[DatabaseGenerated(Identity)]
	public int Silver { get; private init; } = silver;

	[DatabaseGenerated(Identity)]
	public int Gold { get; private init; } = gold;

	[DatabaseGenerated(Identity)]
	public int Platinum { get; private init; } = platinum;
}

public sealed class LegacyKarmaConfiguration : IEntityTypeConfiguration<LegacyKarma>
{
	public void Configure(EntityTypeBuilder<LegacyKarma> builder)
	{
		builder.HasKey(b => new {b.UserId, b.Year});

		builder.Property(p => p.Upvotes).HasDefaultValue(0);
		builder.Property(p => p.Downvotes).HasDefaultValue(0);
		builder.Property(p => p.Silver).HasDefaultValue(0);
		builder.Property(p => p.Gold).HasDefaultValue(0);
		builder.Property(p => p.Platinum).HasDefaultValue(0);
	}
}