using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WingTechBot.Database.Models;

///Represents how much karma someone has sent to someone else.
public sealed class Karma : Model
{
	[Key]
	public int Id { get; init; }

	[Required]
	public string Giver { get; init; }

	[Required]
	public string Receiver { get; init; }

	[Required]
	public int Amount { get; init; }
}

public sealed class KarmaConfiguration : IEntityTypeConfiguration<Karma>
{
	public void Configure(EntityTypeBuilder<Karma> builder)
	{
		builder.Property(e => e.Id).ValueGeneratedOnAdd();
	}
}