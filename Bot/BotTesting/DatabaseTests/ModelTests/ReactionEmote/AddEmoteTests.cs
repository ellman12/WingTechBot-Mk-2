namespace BotTesting.DatabaseTests.ModelTests.ReactionEmote;
using ReactionEmote=WingTechBot.Database.Models.ReactionEmote;

public sealed class AddEmoteTests : ModelTests
{
	private static readonly TestCaseData[] ValidEmotes =
	[
		new("upvote", 123456ul),
		new(":eyes:", null),
		new("👀", null)
	];

	private static readonly TestCaseData[] InvalidEmotes =
	[
		new("jdfhjkadsfhjkdsaf", 0ul),
		new("cheese", 0ul),
		new("jsfjhdsfjkdsfj", null),
		new("", null),
		new("", 374897328ul),
		new("1111111111", null)
	];

	[Test, TestCaseSource(nameof(ValidEmotes))]
	public async Task EmoteDoesntExist(string name, ulong? discordEmoteId)
	{
		await ReactionEmote.AddEmote(name, discordEmoteId);
		Assert.NotNull(ReactionEmote.Find(name, discordEmoteId));
	}

	[Test, TestCaseSource(nameof(ValidEmotes))]
	public async Task FailsForExistingEmote(string name, ulong? discordEmoteId)
	{
		await ReactionEmote.AddEmote(name, discordEmoteId);
		Assert.ThrowsAsync<ArgumentException>(async () => await ReactionEmote.AddEmote(name, discordEmoteId));
	}

	[Test, TestCaseSource(nameof(InvalidEmotes))]
	public void FailsForInvalidEmotes(string name, ulong? discordEmoteId)
	{
		Assert.ThrowsAsync<ArgumentException>(async () => await ReactionEmote.AddEmote(name, discordEmoteId));
	}
}