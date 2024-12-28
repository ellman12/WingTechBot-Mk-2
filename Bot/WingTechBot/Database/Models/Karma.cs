namespace WingTechBot.Database.Models;

///Represents how much karma someone has sent to someone else.
public sealed class Karma(string giver, string receiver, int amount) : Model
{
	[Key]
	public int Id { get; init; }

	[Required]
	public string Giver { get; init; } = giver;

	[Required]
	public string Receiver { get; init; } = receiver;

	[Required]
	public int Amount { get; set; } = amount;

	///Finds the row with this giver and receiver.
	public static async Task<Karma> FindUserPair(string giver, string receiver)
	{
		if (String.IsNullOrWhiteSpace(giver) || String.IsNullOrWhiteSpace(receiver))
			throw new ArgumentException("Username is required");

		if (giver == receiver)
			throw new ArgumentException("Giver and receiver are identical");
		
		await using BotDbContext context = new();
		return await context.Karma.FirstOrDefaultAsync(karma => karma.Giver == giver && karma.Receiver == receiver);
	}

	///Modifies how much karma a user pair has, first creating it if necessary.
	public static async Task GiveKarma(string giver, string receiver, int change)
	{
		if (change == 0)
			return;

		if (String.IsNullOrWhiteSpace(giver) || String.IsNullOrWhiteSpace(receiver))
			throw new ArgumentException("Username is required");

		if (giver == receiver)
			throw new ArgumentException("Giver and receiver are identical");
		
		await using BotDbContext context = new();
		Karma userPair = await FindUserPair(giver, receiver);

		if (userPair == null)
		{
			await context.Karma.AddAsync(new Karma(giver, receiver, change));
		}
		else
		{
			userPair.Amount += change;
		}

		await context.SaveChangesAsync();
	}
}

public sealed class KarmaConfiguration : IEntityTypeConfiguration<Karma>
{
	public void Configure(EntityTypeBuilder<Karma> builder)
	{
		builder.Property(e => e.Id).ValueGeneratedOnAdd();
	}
}