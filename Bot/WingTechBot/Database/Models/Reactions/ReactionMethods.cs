namespace WingTechBot.Database.Models.Reactions;

public sealed partial class Reaction
{
	public static async Task<Reaction> Find(ulong giverId, ulong receiverId, ulong messageId, int emoteId)
	{
		await using BotDbContext context = new();
		return await context.Reactions
			.Include(r => r.Emote)
			.FirstOrDefaultAsync(e => e.GiverId == giverId && e.ReceiverId == receiverId && e.MessageId == messageId && e.EmoteId == emoteId);
	}

	public static async Task<Reaction> AddReaction(ulong giverId, ulong receiverId, ulong messageId, string emoteName, ulong? discordEmoteId)
	{
		if (giverId == 0 || receiverId == 0 || messageId == 0) throw new ArgumentException("Invalid ID");
		if (discordEmoteId == null && !Emoji.TryParse(emoteName, out Emoji _)) throw new ArgumentException("Invalid emoji name");
		if (discordEmoteId != null && Emoji.TryParse(emoteName, out Emoji _)) throw new ArgumentException("Emoji cannot have a Discord emote ID");
		if (String.IsNullOrWhiteSpace(emoteName)) throw new ArgumentException("Invalid emote name");

		var emote = await ReactionEmote.Find(emoteName, discordEmoteId) ?? await ReactionEmote.AddEmote(emoteName, discordEmoteId);

		if (await Find(giverId, receiverId, messageId, emote.Id) != null)
			throw new ArgumentException("Reaction exists");

		await using BotDbContext context = new();
		Reaction reaction = new(giverId, receiverId, messageId, emote.Id);
		await context.Reactions.AddAsync(reaction);
		await context.SaveChangesAsync();
		return reaction;
	}

	public static async Task RemoveReaction(ulong giverId, ulong receiverId, ulong messageId, string emoteName, ulong? discordEmoteId)
	{
		if (giverId == 0 || receiverId == 0 || messageId == 0) throw new ArgumentException("Invalid ID");
		if (discordEmoteId == null && !Emoji.TryParse(emoteName, out Emoji _)) throw new ArgumentException("Invalid emoji name");
		if (discordEmoteId != null && Emoji.TryParse(emoteName, out Emoji _)) throw new ArgumentException("Emoji cannot have a Discord emote ID");
		if (String.IsNullOrWhiteSpace(emoteName)) throw new ArgumentException("Invalid emote name");

		var emote = await ReactionEmote.Find(emoteName, discordEmoteId) ?? throw new ArgumentException("Emote does not exist");

		if (await Find(giverId, receiverId, messageId, emote.Id) == null)
			throw new ArgumentException("Reaction does not exist");

		await using BotDbContext context = new();
		var reaction = await Find(giverId, receiverId, messageId, emote.Id);
		context.Reactions.Remove(reaction);
		await context.SaveChangesAsync();
	}

	public static async Task RemoveAllReactions(ulong messageId)
	{
		if (messageId == 0) throw new ArgumentException("Invalid message ID");

		await using BotDbContext context = new();

		if (await context.Reactions.CountAsync(r => r.MessageId == messageId) == 0)
			throw new ArgumentException($"No messages with id {messageId} to remove reactions from");

		context.Reactions.RemoveRange(context.Reactions.Where(r => r.MessageId == messageId));
		await context.SaveChangesAsync();
	}

	public static async Task RemoveReactionsForEmote(ulong messageId, string emoteName, ulong? discordEmoteId)
	{
		if (messageId == 0) throw new ArgumentException("Invalid message ID");
		if (discordEmoteId == null && !Emoji.TryParse(emoteName, out Emoji _)) throw new ArgumentException("Invalid emoji name");
		if (discordEmoteId != null && Emoji.TryParse(emoteName, out Emoji _)) throw new ArgumentException("Emoji cannot have a Discord emote ID");
		if (String.IsNullOrWhiteSpace(emoteName)) throw new ArgumentException("Invalid emote name");
		
		await using BotDbContext context = new();

		if (await context.Reactions.CountAsync(r => r.MessageId == messageId) == 0)
			throw new ArgumentException($"No messages with id {messageId} to remove reactions from");

		context.Reactions.RemoveRange(context.Reactions.Where(r => r.MessageId == messageId && r.Emote.Name == emoteName && r.Emote.DiscordEmoteId == discordEmoteId));
		await context.SaveChangesAsync();	
	}
}