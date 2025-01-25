namespace ModelTests.ReactionEmoteTests;

[TestFixture]
public sealed class SetKarmaValueTests : ModelTests
{
	private static readonly TestCaseData[] ValidEmotes =
	[
		new("upvote", 123ul, 0, 1),
		new(":eyes:", null, 0, 0),
		new("🤩", null, 3, 20),
	];

	[TestCaseSource(nameof(ValidEmotes))]
	public async Task ReactionEmote_SetKarmaValue(string name, ulong? discordEmoteId, int originalValue, int newValue)
	{
		name = ReactionEmote.ConvertEmojiName(name);
		await using BotDbContext context = new();

		var emote = await ReactionEmote.AddEmote(name, discordEmoteId);
		Assert.NotNull(await ReactionEmote.Find(name, discordEmoteId));

		await emote.SetKarmaValue(newValue);

		emote = await ReactionEmote.Find(name, discordEmoteId);
		Assert.AreEqual(emote.KarmaValue, newValue);
	}
}