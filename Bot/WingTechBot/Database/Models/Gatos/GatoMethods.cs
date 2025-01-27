namespace WingTechBot.Database.Models.Gatos;

public sealed partial class Gato
{
	public static async Task<Gato> Find(string url, string name, ulong uploaderId)
	{
		await using BotDbContext context = new();
		return await context.Gatos.FirstOrDefaultAsync(g => g.Url == url && g.Name == name && g.UploaderId == uploaderId);
	}

	public static async Task<Gato> AddGato(string url, string name, ulong uploaderId)
	{
		if (!IsValidUrl(url)) throw new ArgumentException("Invalid url");
		if (uploaderId == 0) throw new ArgumentException("Invalid uploaderId");

		if (await Find(url, name, uploaderId) != null)
			throw new ArgumentException("Gato exists");

		await using BotDbContext context = new();
		Gato gato = new(url, name, uploaderId);
		await context.Gatos.AddAsync(gato);
		await context.SaveChangesAsync();
		return gato;
	}

	public static bool IsValidUrl(string url)
	{
		return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
	}
}