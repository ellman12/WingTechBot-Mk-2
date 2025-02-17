namespace ModelTests.ReactionEmoteTests;

[TestFixture]
public sealed class ReactionRelationshipTests : ModelTests
{
	[TestCase("upvote", 123456ul, 123ul, 456ul, 867ul, 789ul)]
	public async Task FindReferencedReactions(string emoteName, ulong discordEmoteId, ulong giverId, ulong receiverId, ulong channelId, ulong messageId)
	{
		await ReactionEmote.AddEmote(emoteName, discordEmoteId);
		var newEmote = await ReactionEmote.Find(emoteName, discordEmoteId);
		Assert.NotNull(newEmote);

		foreach (int i in Enumerable.Range(1, 4))
		{
			messageId++;
			await Reaction.AddReaction(giverId, receiverId, channelId, messageId, emoteName, discordEmoteId);
			newEmote = await ReactionEmote.Find(emoteName, discordEmoteId); //Refresh its data.
			Assert.True(newEmote != null && newEmote.Reactions.Count == i);
		}
	}
}