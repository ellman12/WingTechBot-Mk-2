namespace WingTechBot.Database.Models.Reactions;

///Interfaces with <see cref="Reaction"/> to calculate karma for users.
public static class Karma
{
	public static async Task<(ulong receiverId, int karma)[]> GetKarmaLeaderboard(int year)
	{
		await using BotDbContext context = new();
		return context.Reactions
			.Include(r => r.Emote)
			.Where(r => r.Emote.KarmaValue != 0 && r.GiverId != r.ReceiverId && r.CreatedAt.Year == year)
			.GroupBy(r => r.ReceiverId)
			.AsEnumerable()
			.Select(g => (receiverId: g.Key, karma: g.Sum(e => e.Emote.KarmaValue)))
			.OrderByDescending(r => r.karma)
			.ToArray();
	}
}