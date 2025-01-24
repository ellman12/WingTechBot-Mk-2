namespace WingTechBot.Commands.Reactions;

///Watches for reaction events and acts upon those events.
public sealed class ReactionTracker
{
	public void SetUp(WingTechBot bot)
	{
		wingTechBot = bot;
		wingTechBot.Client.ReactionAdded += OnReactionAdded;
		wingTechBot.Client.ReactionRemoved += OnReactionRemoved;
		wingTechBot.Client.ReactionsCleared += OnReactionsCleared;
		wingTechBot.Client.ReactionsRemovedForEmote += OnReactionsRemovedForEmote;
		wingTechBot.Client.MessageDeleted += OnMessageDeleted;
	}

	private WingTechBot wingTechBot;

	private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
	{
		var cachedMessage = await message.GetOrDownloadAsync();
		var name = reaction.Emote.Name;

		if (!IsSupportedEmote(reaction))
		{
			Logger.LogLine($"Ignoring unsupported reaction emote {name}");
			return;
		}

		if (cachedMessage.CreatedAt.Date < wingTechBot.Config.StartDate)
		{
			Logger.LogLine($"Ignoring new reaction {name} added to message before start date");
			return;
		}

		await Reaction.AddReaction(reaction.UserId, cachedMessage.Author.Id, message.Id, name, reaction.Emote is Emote e ? e.Id : null);
	}

	private async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
	{
		var cachedMessage = await message.GetOrDownloadAsync();

		if (cachedMessage.CreatedAt.Date < wingTechBot.Config.StartDate)
		{
			Logger.LogLine($"Message too old, ignoring removal of reaction {reaction.Emote.Name}");
			return;
		}

		await Reaction.RemoveReaction(reaction.UserId, cachedMessage.Author.Id, message.Id, reaction.Emote.Name, reaction.Emote is Emote e ? e.Id : null);
	}

	private async Task OnReactionsCleared(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
	{
		var cachedMessage = await message.GetOrDownloadAsync();

		if (cachedMessage.CreatedAt.Date < wingTechBot.Config.StartDate)
		{
			Logger.LogLine("Message too old, ignoring removal of all reactions");
			return;
		}

		await Reaction.RemoveAllReactions(message.Id);
	}

	private async Task OnReactionsRemovedForEmote(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, IEmote emote)
	{
		var cachedMessage = await message.GetOrDownloadAsync();

		if (cachedMessage.CreatedAt.Date < wingTechBot.Config.StartDate)
		{
			Logger.LogLine($"Message too old, ignoring removal of reactions for emote {emote.Name}");
			return;
		}

		await Reaction.RemoveReactionsForEmote(message.Id, emote.Name, emote is Emote e ? e.Id : null);
	}

	//TODO
	private async Task OnMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
	{
		throw new NotImplementedException();
	}

	private bool IsSupportedEmote(SocketReaction reaction)
	{
		if (Emoji.TryParse(reaction.Emote.Name, out Emoji _))
			return true;

		return reaction.Emote is Emote emote && wingTechBot.Guild.Emotes.Any(e => e.Id == emote.Id);
	}
}