namespace WingTechBot.Database.Models.Gatos;

///Represents a picture of a cat which can be displayed by invoking /gato.
public sealed partial class Gato(byte[] media, string filename, string name, ulong uploaderId) : Model
{
	[Key]
	public int Id { get; private init; }

	[Required]
	public byte[] Media { get; private init; } = media;

	[Required]
	public string Filename { get; private init; } = filename;

	[Required]
	public string Name { get; private init; } = name;

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