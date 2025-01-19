namespace WingTechBot.Database.Models;

public sealed partial class ReactionEmote
{
	///Attempts to convert an emoji code like :eyes: or :thumbsup: into 👀 or 👍.
	public static string ConvertEmojiName(string name)
	{
		if (Emoji.TryParse(name, out Emoji emoji))
			name = emoji.Name;

		return name;
	}

	public static async Task<ReactionEmote> Find(string name, ulong? discordEmoteId)
	{
		await using BotDbContext context = new();
		return await context.ReactionEmotes
			.Include(re => re.Reactions)
			.FirstOrDefaultAsync(e => e.Name == name && e.DiscordEmoteId == discordEmoteId);
	}

	public static async Task<ReactionEmote> AddEmote(string name, ulong? discordEmoteId, int karmaValue = 0)
	{
		if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException("Invalid emote name");
		if (discordEmoteId == null && !Emoji.TryParse(name, out Emoji _)) throw new ArgumentException("Invalid emoji name");
		if (discordEmoteId == 0) throw new ArgumentException("Invalid discord emote id");

		//Ensure storing emoji names properly.
		if (discordEmoteId == null)
			name = ConvertEmojiName(name);

		var existing = await Find(name, discordEmoteId);
		if (existing != null)
			throw new ArgumentException("Emote exists in ReactionEmote table");

		await using BotDbContext context = new();
		ReactionEmote emote = new(name, discordEmoteId, karmaValue);
		await context.ReactionEmotes.AddAsync(emote);
		await context.SaveChangesAsync();
		return emote;
	}
}