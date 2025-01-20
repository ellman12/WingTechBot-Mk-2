namespace WingTechBot.Database.Models.Reactions;

///Represents a Discord emote used for the karma and reaction tracker systems.
public sealed partial class ReactionEmote(string name, ulong? discordEmoteId, int karmaValue = 0) : Model
{
	[Key]
	public int Id { get; private init; }

	///The name of the emote, such as 'upvote' or '👀'.
	[Required]
	public string Name { get; private init; } = name;

	public ulong? DiscordEmoteId { get; private init; } = discordEmoteId;

	///How much karma is given or received when a message receives this emote as a reaction.
	public int KarmaValue { get; private set; } = karmaValue;

	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public DateTime CreatedAt { get; private init; }

	public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();

	///See: https://docs.discordnet.dev/guides/emoji/emoji.html
	[NotMapped]
	public IEmote Parsed => DiscordEmoteId == null ? Emoji.Parse(Name) : Emote.Parse($"<:{Name}:{DiscordEmoteId}>");

	///Convert this ReactionEmote into a string that Discord can interpret.
	public override string ToString() => DiscordEmoteId == null ? Name : $"<:{Name}:{DiscordEmoteId}>";
}

public sealed class ReactionEmoteConfiguration : IEntityTypeConfiguration<ReactionEmote>
{
	public void Configure(EntityTypeBuilder<ReactionEmote> builder)
	{
		builder.Property(e => e.Id).ValueGeneratedOnAdd();
		builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
	}
}