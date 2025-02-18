namespace TestingUtilities.Seeders;

public static class ReactionSeeder
{
	private static readonly Dictionary<string, ulong?> ReactionEmotes = new()
	{
		{"upvote", 111},
		{"downvote", 222},
		{"👍🏼", null},
		{"🤓", null},
		{"😼", null},
		{"🥐", null},
		{"silver", 333},
		{"gold", 444},
		{"platinum", 555},
		{"🥵", null},
		{"🚽", null}
	};

	private static Random random;

	public static async Task Seed(int seed, int minMessages, int maxMessages, int maxReactsPerMessage, int minUsers, int maxUsers)
	{
		const long MaxIdValue = 300;
		
		random = new Random(seed);

		ulong[] userIds = Enumerable.Range(0, random.Next(minUsers, maxUsers)).Select(_ => (ulong)random.NextInt64(MaxIdValue)).ToArray();
		int totalMessages = random.Next(minMessages, maxMessages);

		foreach (int m in Enumerable.Range(0, totalMessages))
		{
			ulong giverId = userIds[random.Next(0, userIds.Length)];
			ulong receiverId = userIds[random.Next(0, userIds.Length)];
			ulong channelId = (ulong)random.NextInt64(0, MaxIdValue);
			ulong messageId = (ulong)random.NextInt64(1, MaxIdValue);

			var emotes = PickUniqueEmotes(random.Next(maxReactsPerMessage));
			foreach (var emote in emotes)
			{
				await Reaction.AddReaction(giverId, receiverId, channelId, messageId, emote.Key, emote.Value);
			}
		}
	}

	private static Dictionary<string, ulong?> PickUniqueEmotes(int amount)
	{
		return ReactionEmotes
			.OrderBy(_ => random.Next())
			.Take(amount)
			.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
	}
}