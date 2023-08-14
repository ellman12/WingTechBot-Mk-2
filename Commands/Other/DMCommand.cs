namespace WingTechBot.Commands.Other;
internal class DMCommand : Command
{
	private string _sendMessage = string.Empty;

	public override void Execute()
	{
		var messageWords = arguments[2..];
		foreach (var s in messageWords)
		{
			_sendMessage += $"{s} ";
		}

		requested.GetOrCreateDMChannelAsync().Result.SendMessageAsync(_sendMessage);
		message.Channel.SendMessageAsync("Sent.");
	}

	public override string LogString => $"DM'd {requested.Username}#{requested.Discriminator}: {_sendMessage}";
	public override bool GetRequested => true;
	public override bool OwnerOnly => true;
}
