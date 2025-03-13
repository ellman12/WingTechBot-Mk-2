namespace WingTechBot.Commands.VC;

public sealed class RefreshSoundsCommand : SlashCommand
{
	public override bool Defer => false;

	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder().WithName("refresh-sounds").WithDescription("Refreshes the available soundboard sounds");
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		await Bot.VoiceChannelConnection.GetSounds();
		await command.RespondAsync("Soundboard sounds have been refreshed", ephemeral: true);
	}
}