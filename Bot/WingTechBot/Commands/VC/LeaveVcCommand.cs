namespace WingTechBot.Commands.VC;

public sealed class LeaveVcCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder().WithName("leave-vc").WithDescription("Make WTB leave the VC channel it's in");
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		var channel = Bot.VoiceChannelConnection.ConnectedChannel;
		if (channel == null)
		{
			await command.FollowupAsync("Not currently in a VC");
		}
		else
		{
			await command.FollowupAsync($"Leaving {channel.Mention}");
			await Bot.VoiceChannelConnection.Disconnect();
		}
	}
}