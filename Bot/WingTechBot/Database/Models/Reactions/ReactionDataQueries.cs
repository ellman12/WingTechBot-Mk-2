namespace WingTechBot.Database.Models.Reactions;

public sealed partial class Reaction
{
	///Gets each reaction a user has received and the amount of each, including legacy karma.
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
}