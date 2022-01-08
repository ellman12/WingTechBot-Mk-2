namespace WingTechBot;
using System;
using System.Collections.Generic;
using System.Threading;
using Discord;
using Discord.WebSocket;

public abstract class Game
{
	public ulong GamemasterID { get; set; }

	private static readonly List<ulong> _list = new();

	public List<ulong> PlayerIDs { get; private set; } = _list;

	protected IMessage LastMessage { get; private set; }

	protected ISocketMessageChannel GameChannel
	{
		get => Program.GetChannel(gameChannelID) as ISocketMessageChannel;
		set => gameChannelID = value.Id;
	}
	protected ulong gameChannelID;

	protected virtual bool Debug => false;
	protected virtual PromptMode AllowedChannels => PromptMode.Any;
	public virtual uint? MaxPlayers { get; protected set; } = null;

	private readonly EventWaitHandle _waitHandle = new(false, EventResetMode.ManualReset);
	private readonly object _messageLock = new();

	private bool _init = false;

	protected List<IMessage> sentMessages = new();
	protected List<IMessage> receivedMessages = new();

	protected virtual Dictionary<string, Action<IMessage, string[]>> Commands { get; }

	protected virtual void Start() { }

	public void Init(IMessage initMessage)
	{
		if (!ModeMatch(initMessage, AllowedChannels)) throw new Exception("This game cannot be started in this channel.");

		if (!_init)
		{
			_init = true;
			GamemasterID = initMessage.Author.Id;
			GameChannel = initMessage.Channel as ISocketMessageChannel;
		}
		else throw new Exception("Game.Init can only be called once!");
	}

	protected void GetPlayers()
	{
		while (MaxPlayers is null || PlayerIDs.Count < MaxPlayers)
		{
			IUser foundPlayer;
			while (!Program.TryGetUser(PromptString(GamemasterID, AllowedChannels, message: $"Enter Player {PlayerIDs.Count + 1}, or type \"stop\" to stop adding players"), out foundPlayer))
			{

				if (LastMessage.Content.Trim().ToLower() == "stop") return;
				else if (LastMessage.Content.Trim().ToLower() == "me")
				{
					foundPlayer = Program.GetUser(GamemasterID);
					break;
				}
				else GameChannel.SendMessageAsync($"Player \"{LastMessage.Content}\" not found."); ;
			}

			if (Program.GameHandler.PlayerAvailable(foundPlayer.Id)) PlayerIDs.Add(foundPlayer.Id);
			else GameChannel.SendMessageAsync($"Player \"{foundPlayer.Username}#{foundPlayer.Discriminator}\" is already in a game.");
		}
	}

	public void Run()
	{
		try
		{
			Start();
			GetPlayers();
			RunGame();
		}
		catch (Exception e)
		{
			GameChannel.SendMessageAsync(e.Message);
			if (Debug) GameChannel.SendMessageAsync(e.StackTrace);
		}
		finally
		{
			Program.GameHandler.EndGame(this);
		}
	}

	public abstract void RunGame();

	public virtual void Shutdown() { }

	protected IMessage Prompt(ulong playerID, PromptMode mode, bool channelMatch = false, string message = null, bool saveMessage = false)
	{
		lock (_messageLock)
		{
			IUser player = GetPlayer(playerID);
			if (playerID != GamemasterID && !PlayerIDs.Contains(playerID)) throw new Exception($"Error: {player.Username}#{player.Discriminator} cannot be prompted: not part of game.");
			if (player.IsBot || player.IsWebhook) throw new Exception($"Error: {player.Username}#{player.Discriminator} is a bot or webhook.");

			LastMessage = null;
		}

		if (message is not null)
		{
			IMessage sent =
				mode == PromptMode.DM
				? DM(message, playerID)
				: WriteLine(message);

			if (saveMessage) sentMessages.Add(sent);
		}

		while (LastMessage is null || LastMessage.Author.Id != playerID || !ModeMatch(LastMessage, mode) || (channelMatch && LastMessage.Channel != GameChannel))
		{
			_waitHandle.Reset();
			_waitHandle.WaitOne();
		}

		return LastMessage;
	}

	protected IMessage PromptAny(PromptMode mode, bool channelMatch = false, string message = null, bool saveMessage = false)
	{
		lock (_messageLock)
		{
			LastMessage = null;
		}

		if (message is not null)
		{
			IMessage sent = WriteLine(message);
			if (saveMessage) sentMessages.Add(sent);
		}

		while (LastMessage is null || !PlayerIDs.Contains(LastMessage.Author.Id) || !ModeMatch(LastMessage, mode) || (channelMatch && LastMessage.Channel != GameChannel))
		{
			_waitHandle.Reset();
			_waitHandle.WaitOne();
		}

		return LastMessage;

	}

	protected string PromptString(ulong playerID, PromptMode mode, bool channelMatch = false, string message = null, bool saveMessage = false) => Prompt(playerID, mode, channelMatch, message, saveMessage).Content;
	protected (ulong, string) PromptAnyString(PromptMode mode, bool channelMatch = false, string message = null, bool saveMessage = false)
	{
		var m = PromptAny(mode, channelMatch, message, saveMessage);
		return (m.Author.Id, m.Content);
	}

	protected T Prompt<T>(ulong playerID, PromptMode mode, Predicate<T> condition, bool channelMatch = false, string message = null, bool saveMessage = false)
	{
		bool found = false;
		T foundValue = default;
		IMessage messageReceived = null;
		while (!found)
		{
			try
			{
				messageReceived = Prompt(playerID, mode, channelMatch, message, saveMessage);
				foundValue = (T)Convert.ChangeType(messageReceived.Content, typeof(T));
				found = condition(foundValue);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				// tell player wrong type
			}
		}

		if (saveMessage && messageReceived is not null) receivedMessages.Add(messageReceived);

		return foundValue;
	}

	protected (ulong, T) PromptAny<T>(PromptMode mode, Predicate<T> condition, bool channelMatch = false, string message = null, bool saveMessage = false)
	{
		bool found = false;
		T foundValue = default;
		ulong foundID = 0;
		IMessage messageReceived = null;
		while (!found)
		{
			try
			{
				messageReceived = PromptAny(mode, channelMatch, message, saveMessage);
				foundValue = (T)Convert.ChangeType(messageReceived.Content, typeof(T));
				foundID = messageReceived.Author.Id;
				found = condition(foundValue);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		if (saveMessage && messageReceived is not null) receivedMessages.Add(messageReceived);

		return (foundID, foundValue);
	}

	protected T Prompt<T>(ulong playerID, PromptMode mode, bool channelMatch = false, string message = null, bool saveMessage = false) => Prompt(playerID, mode, (T _) => true, channelMatch, message, saveMessage);
	protected (ulong, T) PromptAny<T>(PromptMode mode, bool channelMatch = false, string message = null, bool saveMessage = false) => PromptAny(mode, (T _) => true, channelMatch, message, saveMessage);

	protected bool PromptYN(ulong playerID, PromptMode mode, bool channelMatch = false, string message = null, bool saveMessage = false) => Prompt(playerID, mode, (string x) => x.Trim().ToLower() is "y" or "n", channelMatch, message, saveMessage).Trim().ToLower() == "y";
	protected bool PromptAnyYN(PromptMode mode, bool channelMatch = false, string message = null, bool saveMessage = false) => PromptAny(mode, (string x) => x.Trim().ToLower() is "y" or "n", channelMatch, message, saveMessage).Item2.Trim().ToLower() == "y";

	protected bool PromptEnd() => PromptAny(AllowedChannels, (string x) => x.Trim().ToLower() is "next" or "end", true, "Type \"next\" to continue or \"end\" to stop playing.", true).Item2.Trim().ToLower() == "end";


	public void ReceiveMessage(IMessage message)
	{
		lock (message)
		{
			LastMessage = message;
		}

		_waitHandle.Set();
	}

	public void ReceiveCommand(IMessage message)
	{
		if (Commands is not null)
		{
			string command;
			string[] arguments;

			arguments = message.Content[2..].Split(' ');
			command = arguments[0].ToUpper();

			if (Commands.TryGetValue(command, out Action<IMessage, string[]> func)) func(message, arguments);
			else Reply(message, $"Command {command} not recognized.");
		}
	}

	private static bool ModeMatch(IMessage message, PromptMode mode) => mode switch
	{
		PromptMode.DM => message.Channel is IDMChannel,
		PromptMode.Public => message.Channel is IGuildChannel,
		PromptMode.Group => message.Channel is IGroupChannel,
		PromptMode.GroupOrPublic => message.Channel is IGroupChannel or IGuildChannel,
		_ => true,
	};

	protected IMessage DM(object x, ulong id)
	{
		IUser user = GetPlayer(id);
		IMessage message = user.GetOrCreateDMChannelAsync().Result.SendMessageAsync(x.ToString()).Result;
		if (Debug) Console.WriteLine($"DM'd: {x} to {user.Username}#{user.Discriminator}");

		return message;
	}

	protected IMessage WriteLine(object x)
	{
		IMessage message = GameChannel.SendMessageAsync(x.ToString()).Result;
		if (Debug) Console.WriteLine($"Sent: {x}");

		return message;
	}

	protected IMessage WriteLine()
	{
		IMessage message = GameChannel.SendMessageAsync("_ _").Result;
		if (Debug) Console.WriteLine();

		return message;
	}

	protected IMessage SaveWriteLine(object x)
	{
		IMessage message = WriteLine(x);
		sentMessages.Add(message);

		return message;
	}

	protected IMessage Reply(IMessage message, object x)
	{
		IMessage sent = message.Channel.SendMessageAsync(x.ToString()).Result;
		if (Debug) Console.WriteLine($"Sent: {x}");

		return sent;
	}

	protected void DeleteSavedSentMessages()
	{
		foreach (IMessage message in sentMessages) message.Channel.DeleteMessageAsync(message);
		sentMessages.Clear();
	}

	protected void DeleteSavedReceivedMessages()
	{
		foreach (IMessage message in receivedMessages) message.Channel.DeleteMessageAsync(message);
		receivedMessages.Clear();
	}

	protected void DeleteSavedMessages()
	{
		DeleteSavedSentMessages();
		DeleteSavedReceivedMessages();
	}

	protected static IUser GetPlayer(ulong id) => Program.GetUser(id);

	public enum PromptMode
	{
		Any,
		DM,
		Public,
		Group,
		GroupOrPublic
	}
}
