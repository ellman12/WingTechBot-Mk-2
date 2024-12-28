namespace BotTesting.DatabaseTests.ModelTests;

[TestFixture]
public class KarmaTests : ModelTest
{
	[SetUp]
	public async Task SetUp()
	{
		await using BotDbContext context = new();
		await context.Database.EnsureDeletedAsync();
		await context.Database.EnsureCreatedAsync();
	}

	[TestCase("", "")]
	[TestCase("user1", "user1")]
	public void FindUserPair_GiverAndReceiverIdentical(string giver, string receiver)
	{
		Assert.ThrowsAsync<ArgumentException>(async () => await Karma.FindUserPair(giver, receiver));
	}

	[TestCase("", "")]
	[TestCase("", "user123")]
	[TestCase("user69420", "")]
	public void FindUserPair_InvalidUsernames(string giver, string receiver)
	{
		Assert.ThrowsAsync<ArgumentException>(async () => await Karma.FindUserPair(giver, receiver));
	}

	[TestCase("user1", "user2", 7)]
	[TestCase("user69", "user420", -20)]
	[TestCase("user24", "user25", 456)]
	[TestCase("user120", "user240", -234)]
	public async Task GiveKarma_CreateNewUserPair(string giver, string receiver, int change)
	{
		await using BotDbContext context = new();
		var userPair = await Karma.FindUserPair(giver, receiver);
		Assert.IsNull(userPair);

		await Karma.GiveKarma(giver, receiver, change);
		userPair = await Karma.FindUserPair(giver, receiver);
		Assert.NotNull(userPair);
		Assert.AreEqual(change, userPair.Amount);
	}

	[TestCase("user1", "user2", 7, 1)]
	[TestCase("user69", "user420", -20, -1)]
	[TestCase("user24", "user25", 456, -1)]
	[TestCase("user120", "user240", -234, 1)]
	public async Task GiveKarma_UserPairExists(string giver, string receiver, int initialKarma, int change)
	{
		await using BotDbContext context = new();
		await Karma.GiveKarma(giver, receiver, initialKarma);
		var userPair = await Karma.FindUserPair(giver, receiver);
		Assert.NotNull(userPair);
		Assert.AreEqual(initialKarma, userPair.Amount);

		await Karma.GiveKarma(giver, receiver, change);
		userPair = await Karma.FindUserPair(giver, receiver);
		Assert.NotNull(userPair);
		Assert.AreEqual(userPair.Amount + change, initialKarma + change);
	}
}