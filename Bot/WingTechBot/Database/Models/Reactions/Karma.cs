namespace WingTechBot.Database.Models.Reactions;

///Interfaces with <see cref="Reaction"/> to calculate karma for users.
public static class Karma
{
	public static async Task<(ulong receiverId, int karma)[]> GetKarmaLeaderboard(int year)
	{
		await using BotDbContext context = new();

		//Performs an OUTER JOIN
		//TODO: this is very ugly and if we have to do this again, this should go in a custom extension method

		var reactionKarma = context.Reactions
			.Include(r => r.Emote)
			.Where(r => r.Emote.KarmaValue != 0 && r.GiverId != r.ReceiverId && r.CreatedAt.Year == year)
			.GroupBy(r => r.ReceiverId)
			.Select(reactions => new
			{
				id = reactions.Key,
				karma = reactions.Sum(e => e.Emote.KarmaValue)
			});

		var legacyKarma = context.LegacyKarma
			.Where(lk => lk.Year == year)
			.GroupBy(lk => lk.UserId)
			.Select(legacy => new
			{
				id = legacy.Key,
				karma = legacy.Sum(lk => lk.Upvotes - lk.Downvotes)
			});

		return await reactionKarma
			.Union(legacyKarma)
			.GroupBy(k => k.id)
			.Select(k => new
			{
				id = k.Key,
				karma = k.Sum(x => x.karma)
			})
			.OrderByDescending(k => k.karma)
			.AsAsyncEnumerable()
			.Select(k => (receiverId: k.id, k.karma))
			.ToArrayAsync();
	}
}