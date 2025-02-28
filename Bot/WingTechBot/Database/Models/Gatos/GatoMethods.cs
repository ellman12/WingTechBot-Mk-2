namespace WingTechBot.Database.Models.Gatos;

public sealed partial class Gato
{
	public static async Task<Gato> AddGato(byte[] media, string filename, string name, ulong uploaderId)
	{
		if (media == null || media.Length == 0) throw new ArgumentException("Invalid media");
		if (String.IsNullOrWhiteSpace(filename)) throw new ArgumentException("Filename cannot be empty");
		if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");
		if (uploaderId == 0) throw new ArgumentException("Invalid uploaderId");

		await using BotDbContext context = new();
		Gato gato = new(media, filename, name.ToLower(), uploaderId);
		await context.Gatos.AddAsync(gato);
		await context.SaveChangesAsync();
		return gato;
	}

	public static async Task<(string name, int count)[]> GetGatoLeaderboard()
	{
		await using BotDbContext context = new();
		return await context.Gatos
			.GroupBy(g => g.Name)
			.AsAsyncEnumerable()
			.Select(g => (name: String.IsNullOrWhiteSpace(g.Key) ? "No name" : g.Key, count: g.Count()))
			.OrderByDescending(g => g.count)
			.ToArrayAsync();
	}
}