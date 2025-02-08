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
			.Where(r => r.ReceiverId == receiverId && r.GiverId != r.ReceiverId && r.Emote.CreatedAt.Year == year)
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

	///Gets each reaction a user has received from a user or role, and the amount of each, including legacy karma and ignoring self-reactions (unless you mention yourself).
	public static async Task<Dictionary<ReactionEmote, int>> GetReactionsFromUsersForYear(ulong[] giverIds, ulong receiverId, int year)
	{
		await using BotDbContext context = new();
		var legacyKarma = await context.LegacyKarma.FirstOrDefaultAsync(lk => lk.UserId == receiverId && lk.Year == year);
		var userKarma = legacyKarma == null ? new Dictionary<ReactionEmote, int>() : legacyKarma.ConvertEmotes();

		Expression<Func<Reaction, bool>> filter;
		if (giverIds.Length == 1 && giverIds.First() == receiverId)
			filter = reaction => reaction.GiverId == receiverId;
		else
			filter = reaction => reaction.GiverId != receiverId;

		return context.Reactions
			.Include(r => r.Emote)
			.Where(r => r.ReceiverId == receiverId && giverIds.Contains(r.GiverId) && r.Emote.CreatedAt.Year == year)
			.Where(filter)
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
}