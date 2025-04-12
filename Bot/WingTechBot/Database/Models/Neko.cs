namespace WingTechBot.Database.Models;

public sealed class Neko(byte[] media, string filename, ulong uploaderId) : Model
{
	[Key]
	public int Id { get; private init; }

	[Required]
	public byte[] Media { get; private init; } = media;

	[Required]
	public string Filename { get; private init; } = filename;

	[Required]
	public ulong UploaderId { get; private init; } = uploaderId;

	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public DateTime CreatedAt { get; private init; }

	public static async Task<Neko> AddNeko(byte[] media, string filename, ulong uploaderId)
	{
		if (media == null || media.Length == 0) throw new ArgumentException("Invalid media");
		if (String.IsNullOrWhiteSpace(filename)) throw new ArgumentException("Filename cannot be empty");
		if (uploaderId == 0) throw new ArgumentException("Invalid uploaderId");

		await using BotDbContext context = new();
		Neko neko = new(media, filename, uploaderId);
		await context.Nekos.AddAsync(neko);
		await context.SaveChangesAsync();
		return neko;
	}
}

public sealed class NekoConfiguration : IEntityTypeConfiguration<Neko>
{
	public void Configure(EntityTypeBuilder<Neko> builder)
	{
		builder.Property(e => e.Id).ValueGeneratedOnAdd();
		builder.Property(e => e.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
	}
}