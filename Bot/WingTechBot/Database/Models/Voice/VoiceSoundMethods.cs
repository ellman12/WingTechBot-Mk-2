namespace WingTechBot.Database.Models.Voice;

public sealed partial class SoundboardSound
{
	public static async Task<SoundboardSound> Find(ulong id)
	{
		await using BotDbContext context = new();
		return await context.SoundboardSounds.FirstOrDefaultAsync(r => r.Id == id);
	}

	public static async Task<SoundboardSound> AddSoundboardSound(string name, byte[] audio)
	{
		if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace");
		if (audio.Length == 0) throw new ArgumentException("Audio cannot be empty");

		var newSound = new SoundboardSound(name, audio);
		await using BotDbContext context = new();
		await context.SoundboardSounds.AddAsync(newSound);
		await context.SaveChangesAsync();
		return newSound;
	}

	public static async Task RemoveSoundboardSound(ulong id)
	{
		if (id == 0) throw new ArgumentException("Invalid SoundboardSound id");

		await using BotDbContext context = new();
		var sound = await Find(id);
		context.SoundboardSounds.Remove(sound);
		await context.SaveChangesAsync();
	}
}