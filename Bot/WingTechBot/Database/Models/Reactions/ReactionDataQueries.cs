namespace WingTechBot.Database.Models.Reactions;

public sealed partial class Reaction
{
	///Gets each reaction a user has received and the amount of each, including legacy karma and ignoring self-reactions.
	public static async Task<Dictionary<ReactionEmote, int>> GetReactionsUserReceived(ulong receiverId, int year)
	{
		await using BotDbContext context = new();
		var legacyKarma = await context.LegacyKarma.FirstOrDefaultAsync(lk => lk.UserId == receiverId && lk.Year == year);
		var userKarma = legacyKarma == null ? new Dictionary<ReactionEmote, int>() : legacyKarma.ConvertEmotes();

		return context.Reactions
			.Include(r => r.Emote)
			.Where(r => r.ReceiverId == receiverId && r.GiverId != r.ReceiverId && r.CreatedAt.Year == year)
			.GroupBy(r => r.EmoteId)
			.AsEnumerable()
			.Select(g => (g.First().Emote, Count: g.Count()))
			.GroupJoin(userKarma, reactions => reactions.Emote.Id, uk => uk.Key.Id, (reactions, uk) => new
			{
				reactions.Emote,
				Count = reactions.Count + uk.DefaultIfEmpty().First().Value
			})
			.ToDictionary(reactions => reactions.Emote, reactions => reactions.Count);
	}

	///Gets each reaction a user has received from a user or role and the amount of each, excluding legacy karma and self-reactions (unless you mention yourself).
	public static async Task<Dictionary<ReactionEmote, int>> GetReactionsFromUsersForYear(ulong[] giverIds, ulong receiverId, int year)
	{
		await using BotDbContext context = new();

		Expression<Func<Reaction, bool>> filter;
		if (giverIds.Length == 1 && giverIds.First() == receiverId)
			filter = reaction => reaction.GiverId == receiverId;
		else
			filter = reaction => reaction.GiverId != receiverId;

		return context.Reactions
			.Include(r => r.Emote)
			.Where(r => r.ReceiverId == receiverId && giverIds.Contains(r.GiverId) && r.CreatedAt.Year == year)
			.Where(filter)
			.GroupBy(r => r.EmoteId)
			.AsEnumerable()
			.Select(g => (g.First().Emote, Count: g.Count()))
			.ToDictionary(reactions => reactions.Emote, reactions => reactions.Count);
	}

	///Gets each reaction a user has given and the amount of each, including legacy karma and ignoring self-reactions.
	public static async Task<Dictionary<ReactionEmote, int>> GetReactionsUserGiven(ulong giverId, int year)
	{
		await using BotDbContext context = new();
		var legacyKarma = await context.LegacyKarma.FirstOrDefaultAsync(lk => lk.UserId == giverId && lk.Year == year);
		var userKarma = legacyKarma == null ? new Dictionary<ReactionEmote, int>() : legacyKarma.ConvertEmotes();

		return context.Reactions
			.Include(r => r.Emote)
			.Where(r => r.GiverId == giverId && r.GiverId != r.ReceiverId && r.CreatedAt.Year == year)
			.GroupBy(r => r.EmoteId)
			.AsEnumerable()
			.Select(g => (g.First().Emote, Count: g.Count()))
			.GroupJoin(userKarma, reactions => reactions.Emote.Id, uk => uk.Key.Id, (reactions, uk) => new
			{
				reactions.Emote,
				Count = reactions.Count + uk.DefaultIfEmpty().First().Value
			})
			.ToDictionary(reactions => reactions.Emote, reactions => reactions.Count);
	}

	///<summary>Gets each reaction a user has given and the amount of each, excluding self-reactions (unless you mention yourself) and legacy karma.</summary>
	///<remarks>Legacy karma is ignored as it's impossible to calculate this.</remarks>
	public static async Task<Dictionary<ReactionEmote, int>> GetReactionsGivenToUsersForYear(ulong giverId, ulong[] receiverIds, int year)
	{
		await using BotDbContext context = new();

		Expression<Func<Reaction, bool>> filter;
		if (receiverIds.Length == 1 && receiverIds.First() == giverId)
			filter = reaction => reaction.ReceiverId == giverId;
		else
			filter = reaction => reaction.ReceiverId != giverId;

		return context.Reactions
			.Include(r => r.Emote)
			.Where(r => r.GiverId == giverId && receiverIds.Contains(r.ReceiverId) && r.CreatedAt.Year == year)
			.Where(filter)
			.GroupBy(r => r.EmoteId)
			.AsEnumerable()
			.Select(g => (g.First().Emote, Count: g.Count()))
			.ToDictionary(reactions => reactions.Emote, reactions => reactions.Count);
	}

	///Calculates which emotes have been used the most in reactions, including self-reactions.
	public static async Task<Dictionary<ReactionEmote, int>> GetEmoteLeaderboardForYear(int year)
	{
		await using BotDbContext context = new();

		return context.Reactions
			.Include(r => r.Emote)
			.Where(r => r.CreatedAt.Year == year)
			.GroupBy(r => r.EmoteId)
			.AsEnumerable()
			.Select(g => (g.First().Emote, Count: g.Count()))
			.OrderByDescending(g => g.Count)
			.ToDictionary(reactions => reactions.Emote, reactions => reactions.Count);
	}

	///<summary>Returns a selection of your messages that got the most reactions with this emote, excluding legacy karma and self-reactions.</summary>
	///<remarks>Legacy karma is ignored as it's impossible to calculate this.</remarks>
	public static async Task<(ulong messageId, ReactionEmote emote, int count)[]> GetTopMessagesForUser(ulong userId, string emoteName, int amount)
	{
		await using BotDbContext context = new();

		return context.Reactions
			.Include(r => r.Emote)
			.Where(r => r.GiverId != userId && r.ReceiverId == userId && r.Emote.Name == emoteName)
			.GroupBy(r => r.MessageId)
			.AsEnumerable()
			.Select(g => (g.First().MessageId, g.First().Emote, count: g.Count()))
			.OrderByDescending(g => g.count)
			.Take(amount)
			.ToArray();
	}
}