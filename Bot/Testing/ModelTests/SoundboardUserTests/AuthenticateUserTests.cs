namespace ModelTests.SoundboardUserTests;

public sealed class AuthenticateUserTests : ModelTests
{
	[TestCase(123ul, 456ul), TestCase(456ul, 456ul)]
	public async Task UserDoesNotExist(ulong userId, ulong authenticatorId)
	{
		await using BotDbContext context = new();

		Assert.IsEmpty(context.SoundboardUsers);
		Assert.Null(await SoundboardUser.Find(userId));

		await SoundboardUser.AuthenticateUser(userId, authenticatorId);

		Assert.AreEqual(1, context.SoundboardUsers.Count());

		var user = await SoundboardUser.Find(userId);
		Assert.NotNull(user);
		Assert.AreEqual(userId, user.Id);
		Assert.AreEqual(authenticatorId, user.AuthenticatorId);
	}

	[TestCase(123ul, 456ul), TestCase(456ul, 456ul)]
	public async Task IgnoresWhenUserExists(ulong userId, ulong authenticatorId)
	{
		await using BotDbContext context = new();

		await UserDoesNotExist(userId, authenticatorId);

		await SoundboardUser.AuthenticateUser(userId, authenticatorId);
		Assert.AreEqual(1, context.SoundboardUsers.Count());
	}
}