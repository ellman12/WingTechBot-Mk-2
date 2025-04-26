namespace ModelTests.SoundboardUserTests;

public sealed class RevokeUserTests : ModelTests
{
	[TestCase(123ul, 456ul), TestCase(456ul, 456ul)]
	public async Task UserExists(ulong userId, ulong authenticatorId)
	{
		await using BotDbContext context = new();

		await SoundboardUser.AuthenticateUser(userId, authenticatorId);

		Assert.IsNotEmpty(context.SoundboardUsers);
		Assert.NotNull(await SoundboardUser.Find(userId));

		await SoundboardUser.RevokeUser(userId);

		Assert.IsEmpty(context.SoundboardUsers);
		Assert.Null(await SoundboardUser.Find(userId));
	}

	[TestCase(123ul, 456ul), TestCase(456ul, 456ul)]
	public async Task IgnoresWhenUserDoesNotExist(ulong userId, ulong authenticatorId)
	{
		await using BotDbContext context = new();

		Assert.IsEmpty(context.SoundboardUsers);
		Assert.Null(await SoundboardUser.Find(userId));

		await SoundboardUser.RevokeUser(userId);

		Assert.IsEmpty(context.SoundboardUsers);
		Assert.Null(await SoundboardUser.Find(userId));
	}
}