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

		await using BotDbContext context = new();
		ulong id = context.SoundboardSounds.Any() ? context.SoundboardSounds.Max(s => s.Id) + 1 : 1;
		var newSound = new SoundboardSound(id, name, audio);
		await context.SoundboardSounds.AddAsync(newSound);
		await context.SaveChangesAsync();
		return newSound;
	}

	public static async Task OverwriteSoundAudio(string name, byte[] newAudio)
	{
		if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace");
		if (newAudio.Length == 0) throw new ArgumentException("Audio cannot be empty");

		await using BotDbContext context = new();
		var sound = await context.SoundboardSounds.SingleOrDefaultAsync(s => EF.Functions.ILike(s.Name, name));

		if (sound == null)
			throw new ArgumentException($"Sound with name {name} could not be found");

		sound.Audio = newAudio;
		await context.SaveChangesAsync();
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