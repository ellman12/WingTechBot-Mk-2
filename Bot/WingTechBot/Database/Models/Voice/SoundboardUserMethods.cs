namespace WingTechBot.Database.Models.Voice;

public sealed partial class SoundboardUser
{
	public static async Task<SoundboardUser> Find(ulong id)
	{
		await using BotDbContext context = new();
		return await context.SoundboardUsers.FirstOrDefaultAsync(r => r.Id == id);
	}

	public static async Task<SoundboardUser[]> GetAll()
	{
		await using BotDbContext context = new();
		return context.SoundboardUsers.ToArray();
	}

	public static async Task AuthenticateUser(ulong id, ulong authenticatorId)
	{
		await using BotDbContext context = new();

		if (await Find(id) != null)
		{
			return;
		}

		var newUser = new SoundboardUser(id, authenticatorId);
		await context.SoundboardUsers.AddAsync(newUser);
		await context.SaveChangesAsync();
	}

	public static async Task RevokeUser(ulong id)
	{
		await using BotDbContext context = new();

		var user = await Find(id);
		if (user == null)
		{
			return;
		}

		context.SoundboardUsers.Remove(user);
		await context.SaveChangesAsync();
	}
}