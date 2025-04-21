namespace WingTechBot.Commands.VC;

public sealed class LeaveVcCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder().WithName("leave-vc").WithDescription("Make WTB leave the VC channel it's in");
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		var channel = Bot.VoiceChannelConnection.ConnectedChannel;
		if (channel == null)
		{
			await command.FollowupAsync("Not currently in a VC");
		}
		else
		{
			await command.FollowupAsync($"Leaving {channel.Mention}");
			await PlayLeaveSound();
			await Bot.VoiceChannelConnection.Disconnect();
		}
	}

	private async Task PlayLeaveSound()
	{
		if (!Bot.Config.AutoSounds.TryGetValue("UserLeave", out var eventGroup))
			return;

		if (!eventGroup.TryGetValue(Bot.Config.UserId, out var soundIds))
			return;

		var soundId = soundIds.GetRandom();
		var sound = Bot.VoiceChannelConnection.AvailableSounds.Single(s => s.SoundId == soundId);
		Bot.VoiceChannelConnection.PlaySound(sound);

		await Task.Delay(1000);
	}
}