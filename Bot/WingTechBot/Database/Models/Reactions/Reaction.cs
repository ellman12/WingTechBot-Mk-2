namespace WingTechBot.Database.Models.Reactions;

///Represents a reaction to a Discord message.
public sealed partial class Reaction(ulong giverId, ulong receiverId, ulong channelId, ulong messageId, int emoteId) : Model
{
	[Key]
	public int Id { get; private init; }

	[Required]
	public ulong GiverId { get; private init; } = giverId;

	[Required]
	public ulong ReceiverId { get; private init; } = receiverId;

	[Required]
	public ulong ChannelId { get; private init; } = channelId;

	[Required]
	public ulong MessageId { get; private init; } = messageId;

	///The id of the <see cref="ReactionEmote"/> for this Reaction.
	[Required]
	public int EmoteId { get; private init; } = emoteId;

	public ReactionEmote Emote { get; private init; }

	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public DateTime CreatedAt { get; private init; }
}

public sealed class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
{
	public void Configure(EntityTypeBuilder<Reaction> builder)
	{
		builder.Property(e => e.Id).ValueGeneratedOnAdd();
		builder.Property(e => e.CreatedAt).HasDefaultValueSql("timezone('utc', now())");

		//One ReactionEmote can have 0, 1, or more Reactions referencing it.
		builder
			.HasOne(r => r.Emote)
			.WithMany(re => re.Reactions)
			.HasForeignKey(r => r.EmoteId);
	}
}