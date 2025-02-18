namespace ModelTests.ReactionTests;

public abstract class ReactionTests : ModelTests
{
	protected static readonly string KarmaTestsPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName, "ReactionTests/LegacyKarmaTests");

	///Create some reaction rows for testing.
	protected static async Task CreateReactions(int messages, int reactionsPerMessage, ReactionEmote[] emotes, ulong giverId, ulong receiverId, ulong channelId, ulong messageId)
	{
		foreach (int _ in Enumerable.Range(0, messages))
		{
			foreach (int e in Enumerable.Range(0, reactionsPerMessage))
			{
				var emote = emotes[e];
				await Reaction.AddReaction(giverId, receiverId, channelId, messageId, emote.Name, emote.DiscordEmoteId);

				var newEmote = await ReactionEmote.Find(emote.Name, emote.DiscordEmoteId);
				var reaction = await Reaction.Find(giverId, receiverId, channelId, messageId, newEmote.Id);
				Assert.NotNull(newEmote);
				Assert.NotNull(reaction);
			}

			messageId++;
		}
	}
}