using Discord;
using Discord.WebSocket;
using System;
using System.Linq;

namespace WingTechBot
{
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

            if (message.Channel is SocketGuildChannel) userRoles = ((IGuild)(message.Channel as SocketGuildChannel).Guild).GetUserAsync(message.Author.Id).Result.RoleIds.ToArray();

            if (GetRequested)
            {
                requested = Program.GetUserFromMention(message, arguments);
                if (requested == null) throw new ArgumentException($"Command {Name} requires a user to be mentioned.");
            }

            if (GetReply)
            {
                try
                {
                    replied = message.Channel.GetMessageAsync(message.Reference.MessageId.Value).Result;
                }
                catch { }
                if (replied == null) throw new ArgumentException($"Command {Name} must include a reply to another message");
            }

            if (RequiredRoles != null)
            {
                if (message.Channel is SocketGuildChannel)
                {
                    if (message.Author.Id != Secrets.OWNER_USER_ID && !RequiredRoles.Any(x => userRoles.Contains(x)))
                    {
                        throw new Exception($"You do not have sufficient rank to call command {Name}.");
                    }
                }
                else
                {
                    throw new Exception($"Command {Name} cannot be called in DMs.");
                }
            }

            if (OwnerOnly && message.Author.Id != Secrets.OWNER_USER_ID) throw new Exception($"Only {Program.GetUser(Secrets.OWNER_USER_ID).Mention} can call command {Name}.");
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
                Type type = GetType();
                return type.Name[..(type.Name.Length - "COMMAND".Length)];
            }
        }

        public virtual string[] Aliases { get => new string[] { Name.ToLower() }; }
    }
}
