namespace WingTechBot.Commands.Reactions;

///Watches for reaction events and acts upon those events.
public sealed class ReactionTracker
{
	public void SetUp(WingTechBot bot)
	{
		bot.Client.ReactionAdded += OnReactionAdded;
		bot.Client.ReactionRemoved += OnReactionRemoved;
		bot.Client.ReactionsCleared += OnReactionsCleared;
		bot.Client.ReactionsRemovedForEmote += OnReactionsRemovedForEmote;
		bot.Client.MessageDeleted += OnMessageDeleted;
	}

	private static async Task OnReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
	{
		ulong authorId = await GetMessageAuthorId(message, channel);
		await Reaction.AddReaction(reaction.UserId, authorId, message.Id, reaction.Emote.Name, reaction.Emote is Emote e ? e.Id : null);
	}

	private static async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
	{
		ulong authorId = await GetMessageAuthorId(message, channel);
		await Reaction.RemoveReaction(reaction.UserId, authorId, message.Id, reaction.Emote.Name, reaction.Emote is Emote e ? e.Id : null);
	}

	//TODO
	private static async Task OnReactionsCleared(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
	{
		throw new NotImplementedException();
	}

	//TODO
	private static async Task OnReactionsRemovedForEmote(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, IEmote emote)
	{
		throw new NotImplementedException();
	}

	//TODO
	private static async Task OnMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
	{
		throw new NotImplementedException();
	}

	///Tries to get the ID of the user who sent the message via cache, but if the cache is empty, fetch the message first.
	private static async Task<ulong> GetMessageAuthorId(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
	{
		return message.HasValue ? message.Value.Author.Id : (await channel.Value.GetMessageAsync(message.Id)).Author.Id;
	}
}