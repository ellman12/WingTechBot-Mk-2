namespace WingTechBot.Database.Models.Voice;

public sealed partial class VoiceSound
{
	public static async Task<VoiceSound> Find(ulong id)
	{
		await using BotDbContext context = new();
		return await context.VoiceSounds.FirstOrDefaultAsync(r => r.Id == id);
	}

	public static async Task<VoiceSound> AddVoiceSound(string name, byte[] audio)
	{
		if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace");
		if (audio.Length == 0) throw new ArgumentException("Audio cannot be empty");

		var newSound = new VoiceSound(name, audio);
		await using BotDbContext context = new();
		await context.VoiceSounds.AddAsync(newSound);
		await context.SaveChangesAsync();
		return newSound;
	}

	public static async Task RemoveVoiceSound(ulong id)
	{
		if (id == 0) throw new ArgumentException("Invalid VoiceSound id");

		await using BotDbContext context = new();
		var sound = await Find(id);
		context.VoiceSounds.Remove(sound);
		await context.SaveChangesAsync();
	}
}