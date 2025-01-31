namespace WingTechBot.Database.Models.Reactions;

///Interfaces with <see cref="Reaction"/> to calculate karma for users.
public static class Karma
{
	public static async Task<(ulong receiverId, int karma)[]> GetKarmaLeaderboard(int year)
	{
		await using BotDbContext context = new();
		return await context.Reactions
			.Include(r => r.Emote)
			.Where(r => r.Emote.KarmaValue != 0 && r.GiverId != r.ReceiverId && r.CreatedAt.Year == year)
			.GroupBy(r => r.ReceiverId)
			.GroupJoin(context.LegacyKarma, reactions => reactions.Key, lk => lk.UserId, (reactions, legacyKarma) => new
			{
				id = reactions.Key,
				karma = reactions.Sum(e => e.Emote.KarmaValue) + legacyKarma.Where(lk => lk.Year == year).Sum(lk => lk.Upvotes - lk.Downvotes)
			})
			.OrderByDescending(r => r.karma)
			.AsAsyncEnumerable()
			.Select(k => (receiverId: k.id, k.karma))
			.ToArrayAsync();
	}
}