namespace WingTechBot.Commands.Reactions;

///Watches for reaction events and acts upon those events.
public sealed class ReactionTracker
{
	public void SetUp(WingTechBot bot)
	{
		Logger.LogLine("Setting up ReactionTracker");

		wingTechBot = bot;
		wingTechBot.Client.ReactionAdded += OnReactionAdded;
		wingTechBot.Client.ReactionRemoved += OnReactionRemoved;
		wingTechBot.Client.ReactionsCleared += OnReactionsCleared;
		wingTechBot.Client.ReactionsRemovedForEmote += OnReactionsRemovedForEmote;
		wingTechBot.Client.MessageDeleted += OnMessageDeleted;
		//Note, some events, like channel deleted and messages bulk deleted I haven't implemented because they never happen.

		Logger.LogLine("Finish setting up ReactionTracker");
	}

	private WingTechBot wingTechBot;

	private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
	{
		var cachedMessage = await message.GetOrDownloadAsync();
		var cachedChannel = await channel.GetOrDownloadAsync();

		try
		{
			if (cachedMessage == null)
			{
				Logger.LogLine("Skipping reaction added to message without value", LogSeverity.Warning);
				return;
			}

			var name = reaction.Emote.Name;
			var authorId = cachedMessage.Author.Id;
			var wtbId = wingTechBot.Config.UserId;

			if (cachedMessage.CreatedAt.Date < wingTechBot.Config.StartDate)
			{
				Logger.LogLine($"Ignoring new reaction {name} added to message before start date");
				return;
			}

			//These cause issues with integration tests.
			#if NOT_TESTING
			if (reaction.UserId == cachedMessage.Author.Id)
			{
				await ReactionScolding.SendScold(name, cachedChannel, reaction.User.Value);
			}

			//Bot downvotes itself if someone adds a downvote to one of its messages.
			if (authorId == wtbId && name == "downvote")
			{
				await cachedMessage.AddReactionAsync(reaction.Emote);
			}
			#endif

			//If someone downvotes themselves, the bot will also downvote them.
			if (authorId != wtbId && authorId == reaction.UserId && name == "downvote")
			{
				await cachedMessage.AddReactionAsync(reaction.Emote);
			}

			await Reaction.AddReaction(reaction.UserId, authorId, channel.Id, message.Id, name, reaction.Emote is Emote e ? e.Id : null);
		}
		catch (Exception e)
		{
			await Logger.LogExceptionAsMessage(e, cachedChannel);
		}
	}

	private async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
	{
		var cachedMessage = await message.GetOrDownloadAsync();
		var cachedChannel = await channel.GetOrDownloadAsync();

		try
		{
			if (cachedMessage == null)
			{
				Logger.LogLine("Skipping reaction removed from message without value", LogSeverity.Warning);
				return;
			}

			var name = reaction.Emote.Name;

			if (cachedMessage.CreatedAt.Date < wingTechBot.Config.StartDate)
			{
				Logger.LogLine($"Message too old, ignoring removal of reaction {name}");
				return;
			}

			await Reaction.RemoveReaction(reaction.UserId, cachedMessage.Author.Id, cachedChannel.Id, message.Id, name, reaction.Emote is Emote e ? e.Id : null);
		}
		catch (Exception e)
		{
			await Logger.LogExceptionAsMessage(e, cachedChannel);
		}
	}

	private async Task OnReactionsCleared(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
	{
		var cachedMessage = await message.GetOrDownloadAsync();
		var cachedChannel = await channel.GetOrDownloadAsync();

		try
		{
			if (cachedMessage == null)
			{
				Logger.LogLine("Skipping reactions removed from message without value", LogSeverity.Warning);
				return;
			}

			if (cachedMessage.CreatedAt.Date < wingTechBot.Config.StartDate)
			{
				Logger.LogLine("Message too old, ignoring removal of all reactions");
				return;
			}

			await Reaction.RemoveAllReactions(message.Id);
		}
		catch (Exception e)
		{
			await Logger.LogExceptionAsMessage(e, cachedChannel);
		}
	}

	private async Task OnReactionsRemovedForEmote(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, IEmote emote)
	{
		var cachedMessage = await message.GetOrDownloadAsync();
		var cachedChannel = await channel.GetOrDownloadAsync();

		try
		{
			if (cachedMessage == null)
			{
				Logger.LogLine("Skipping reactions removed from message without value", LogSeverity.Warning);
				return;
			}

			if (cachedMessage.CreatedAt.Date < wingTechBot.Config.StartDate)
			{
				Logger.LogLine($"Message too old, ignoring removal of reactions for emote {emote.Name}");
				return;
			}

			await Reaction.RemoveReactionsForEmote(message.Id, emote.Name, emote is Emote e ? e.Id : null);
		}
		catch (Exception e)
		{
			await Logger.LogExceptionAsMessage(e, cachedChannel);
		}
	}

	private async Task OnMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
	{
		var cachedChannel = await channel.GetOrDownloadAsync();

		try
		{
			if (message.Value is not IUserMessage)
				return;

			var cachedMessage = await message.GetOrDownloadAsync();

			if (cachedMessage.CreatedAt.Date < wingTechBot.Config.StartDate)
			{
				Logger.LogLine("Message too old, ignoring removal of all reactions for deleted message");
				return;
			}

			if (!cachedMessage.Reactions.Any())
			{
				Logger.LogLine($"No reactions to remove from message {cachedMessage.Id}", LogSeverity.Verbose);
				return;
			}

			await Reaction.RemoveAllReactions(message.Id);
		}
		catch (Exception e)
		{
			await Logger.LogExceptionAsMessage(e, cachedChannel);
		}
	}
}