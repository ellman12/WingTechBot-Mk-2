namespace WingTechBot.Commands.Moderation;
using Discord.WebSocket;

internal class PinCommand : Command
{
	private string _pin;

	public override void Execute()
	{
		try
		{
			replied = message.Channel.GetMessageAsync(message.Reference.MessageId.Value).Result;

			if (replied.IsPinned)
			{
				((SocketUserMessage)replied).UnpinAsync();
			}
			else
			{
				((SocketUserMessage)replied).PinAsync();
			}

			_pin =
				replied.IsPinned
				? "Pin"
				: "Unpin";

			message.Channel.SendMessageAsync($"{_pin}ning message from {replied.Author.Mention}.");
		}
		catch
		{
			throw new($"Failed to {_pin} message.");
		}
	}

	public override string LogString => $"{_pin}ned a message in {replied.Channel.Name}";
	public override bool Audit => true;
	public override ulong[] RequiredRoles => new[] { Program.Config.ModRoleID ?? 0 };
	public override string[] Aliases => new[] { "pin", "unpin", "p", "up" };
	public override bool GetReply => true;
}
