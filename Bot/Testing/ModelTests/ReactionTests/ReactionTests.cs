namespace ModelTests.ReactionTests;

public abstract class ReactionTests : ModelTests
{
	///Create some reaction rows for testing.
	protected static async Task CreateReactions(int messages, int reactionsPerMessage, ReactionEmote[] emotes, ulong giverId, ulong receiverId, ulong messageId)
	{
		foreach (int _ in Enumerable.Range(0, messages))
		{
			foreach (int e in Enumerable.Range(0, reactionsPerMessage))
			{
				var emote = emotes[e];
				await Reaction.AddReaction(giverId, receiverId, messageId, emote.Name, emote.DiscordEmoteId);

				var newEmote = await ReactionEmote.Find(emote.Name, emote.DiscordEmoteId);
				var reaction = await Reaction.Find(giverId, receiverId, messageId, newEmote.Id);
				Assert.NotNull(newEmote);
				Assert.NotNull(reaction);
			}

			messageId++;
		}
	}
}