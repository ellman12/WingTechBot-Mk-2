namespace WingTechBot;
using System;
using System.Linq;
using Discord;
using Discord.WebSocket;

public abstract class Command
{
	protected SocketMessage message;
	protected string[] arguments;
	protected ulong[] userRoles;
	protected IUser requested;
	protected IMessage replied;

	public void Init(SocketMessage message, string[] arguments)
	{
		this.message = message;
		this.arguments = arguments;

		if (message.Channel is SocketGuildChannel)
		{
			userRoles = ((IGuild)(message.Channel as SocketGuildChannel).Guild).GetUserAsync(message.Author.Id).Result.RoleIds.ToArray();
		}

		if (GetRequested)
		{
			requested = Program.GetUserFromMention(message, arguments);
			if (requested is null)
			{
				throw new ArgumentException($"Command {Name} requires a user to be mentioned.");
			}
		}

		if (GetReply)
		{
			try
			{
				replied = message.Channel.GetMessageAsync(message.Reference.MessageId.Value).Result;
			}
			catch { }

			if (replied is null)
			{
				throw new ArgumentException($"Command {Name} must include a reply to another message");
			}
		}

		if (RequiredRoles is not null)
		{
			if (message.Channel is SocketGuildChannel)
			{
				if (message.Author.Id != Program.Config.OwnerID && !RequiredRoles.Any(x => userRoles.Contains(x)))
				{
					throw new($"You do not have sufficient rank to call command {Name}.");
				}
			}
			else
			{
				throw new($"Command {Name} cannot be called in DMs.");
			}
		}

		if (OwnerOnly && message.Author.Id != Program.Config.OwnerID)
		{
			throw new($"Only {Program.GetUser(Program.Config.OwnerID).Mention} can call command {Name}.");
		}
	}

	public abstract void Execute();

	public abstract string LogString { get; }

	public virtual ulong[] RequiredRoles { get; } = null;

	public virtual bool GetRequested { get; } = false;

	public virtual bool GetReply { get; } = false;

	public virtual bool Audit { get; } = false;

	public virtual bool OwnerOnly { get; } = false;

	public string Name
	{
		get
		{
			var type = GetType();
			return type.Name[..(type.Name.Length - "COMMAND".Length)];
		}
	}

	public virtual string[] Aliases => new[] { Name.ToLower() };
}
