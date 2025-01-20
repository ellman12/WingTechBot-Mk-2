namespace BotTesting.DatabaseTests.ModelTests.ReactionTests;

[TestFixture]
public sealed class ReactionEmoteRelationshipTests : ModelTests
{
	[TestCase(123456ul, 123ul, 456ul, "upvote", 565656ul, 1)]
	public async Task FindReferencedReactionEmote(ulong giverId, ulong receiverId, ulong messageId, string emoteName, ulong discordEmoteId, int emoteId)
	{
		await Reaction.AddReaction(giverId, receiverId, messageId, emoteName, discordEmoteId);
		var newReaction = await Reaction.Find(giverId, receiverId, messageId, emoteId);
		Assert.True(newReaction != null && newReaction.Emote != null);
	}
}