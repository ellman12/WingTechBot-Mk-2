namespace WingTechBot.Database.Models;

///Represents a reaction to a Discord message.
public sealed class Reaction(ulong giverId, ulong receiverId, ulong messageId, int emoteId) : Model
{
	[Key]
	public int Id { get; private init; }

	[Required]
	public ulong GiverId { get; private init; } = giverId;

	[Required]
	public ulong ReceiverId { get; private init; } = receiverId;

	[Required]
	public ulong MessageId { get; private init; } = messageId;

	[Required]
	public int EmoteId { get; private init; } = emoteId;

	public ReactionEmote Emote { get; private init; }

	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public DateTime CreatedAt { get; private init; }

	public static async Task<Reaction> Find(ulong giverId, ulong receiverId, ulong messageId, int emoteId)
	{
		await using BotDbContext context = new();
		return await context.Reactions
			.Include(r => r.Emote)
			.FirstOrDefaultAsync(e => e.GiverId == giverId && e.ReceiverId == receiverId && e.MessageId == messageId && e.EmoteId == emoteId);
	}

	public static async Task<Reaction> AddReaction(ulong giverId, ulong receiverId, ulong messageId, int emoteId)
	{
		if (giverId == 0 || receiverId == 0 || messageId == 0 || emoteId == 0)
			throw new ArgumentException("Invalid ID");

		await using BotDbContext context = new();
		Reaction reaction = new(giverId, receiverId, messageId, emoteId);
		await context.Reactions.AddAsync(reaction);
		await context.SaveChangesAsync();
		return reaction;
	}
}

public sealed class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
{
	public void Configure(EntityTypeBuilder<Reaction> builder)
	{
		builder.Property(e => e.Id).ValueGeneratedOnAdd();
		builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

		//One ReactionEmote can have 0, 1, or more Reactions referencing it.
		builder
			.HasOne(r => r.Emote)
			.WithMany(re => re.Reactions)
			.HasForeignKey(r => r.EmoteId);
	}
}