namespace ModelTests.ReactionTests;

[TestFixture]
public sealed class ReactionEmoteRelationshipTests : ReactionTests
{
	[TestCase(123456ul, 123ul, 456ul, 867ul, "upvote", 565656ul, 1)]
	public async Task FindReferencedReactionEmote(ulong giverId, ulong receiverId, ulong channelId, ulong messageId, string emoteName, ulong discordEmoteId, int emoteId)
	{
		await Reaction.AddReaction(giverId, receiverId, channelId, messageId, emoteName, discordEmoteId);
		var newReaction = await Reaction.Find(giverId, receiverId, channelId, messageId, emoteId);
		Assert.True(newReaction != null && newReaction.Emote != null);
	}
}