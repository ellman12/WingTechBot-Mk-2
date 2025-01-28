namespace WingTechBot.Database.Models.Reactions;

public sealed partial class Reaction
{
	///Gets each reaction a user has received and the amount of each.
	public static async Task<(ReactionEmote reactionEmote, int count)[]> GetReactionsUserReceived(ulong receiverId, int? year = null)
	{
		await using BotDbContext context = new();
		return await context.Reactions
			.Include(r => r.Emote)
			.Where(r => r.ReceiverId == receiverId && r.GiverId != r.ReceiverId && (year == null ? r.Emote.CreatedAt.Year > 0 : r.Emote.CreatedAt.Year == year))
			.GroupBy(r => r.EmoteId)
			.AsAsyncEnumerable()
			.Select(g => (g.First().Emote, g.Count()))
			.ToArrayAsync();
	}
}