namespace WingTechBot.Commands.VC;

public sealed class AvailableSoundsCommand : SlashCommand
{
	public override bool Defer => false;

	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("available-sounds")
			.WithDescription("Lists all the sounds that can be played");
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		string message = String.Join('\n', Bot.VoiceChannelConnection.AvailableSounds.Select(s => s.Name).Order());
		await command.RespondAsync(message, ephemeral: true);
	}
}