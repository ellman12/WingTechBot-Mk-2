namespace WingTechBot.Commands.VC;

public sealed class AvailableSoundsCommand : SlashCommand
{
	public override bool Defer => false;

	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder().WithName("available-sounds").WithDescription("Lists all the sounds that can be played");
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		var groups = Bot.VoiceChannelConnection.AvailableSounds.GroupBy(s => s.GuildId);

		string message = "";
		foreach (var group in groups)
		{
			message += $"### {(group.Key == null ? "Default" : Bot.Client.GetGuild(ulong.Parse(group.Key)).Name)}\n";
			message += $"{String.Join("\n", group.Select(s => $"* {s.Name}"))}\n";
		}

		await command.RespondAsync(message, ephemeral: true);
	}
}