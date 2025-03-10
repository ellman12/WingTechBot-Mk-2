namespace WingTechBot.Commands.VC;

public sealed class StopSoundsCommand: SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder().WithName("stop-sounds").WithDescription("Stops all currently-playing sounds.");
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		var sounds = Bot.VoiceChannelConnection.PlayingSounds;
		await command.FollowupAsync(sounds.Any() ? $"Stopping all {sounds.Count} sounds" : "No sounds to stop");
		await Bot.VoiceChannelConnection.CancelSounds();
	}
}