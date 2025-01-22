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
		ulong authorId = (await message.GetOrDownloadAsync()).Author.Id;
		await Reaction.AddReaction(reaction.UserId, authorId, message.Id, reaction.Emote.Name, reaction.Emote is Emote e ? e.Id : null);
	}

	private static async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
	{
		ulong authorId = (await message.GetOrDownloadAsync()).Author.Id;
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
}