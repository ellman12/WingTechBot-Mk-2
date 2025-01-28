namespace WingTechBot.Database.Models.Gatos;

///Represents a picture of a cat which can be displayed by invoking /gato.
public sealed partial class Gato(string url, string name, ulong uploaderId) : Model
{
	[Key]
	public int Id { get; private init; }

	///URL to the media on Discord.
	[Required]
	public string Url { get; private init; } = url;

	///Name of the cat.
	public string Name { get; private init; } = name;

	///Who uploaded the cat media.
	[Required]
	public ulong UploaderId { get; private init; } = uploaderId;

	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public DateTime CreatedAt { get; private init; }
}

public sealed class GatoConfiguration : IEntityTypeConfiguration<Gato>
{
	public void Configure(EntityTypeBuilder<Gato> builder)
	{
		builder.Property(e => e.Id).ValueGeneratedOnAdd();
		builder.Property(e => e.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
	}
}