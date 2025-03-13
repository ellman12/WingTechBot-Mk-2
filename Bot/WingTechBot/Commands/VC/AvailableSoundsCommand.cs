namespace WingTechBot.Commands.VC;

public sealed class AvailableSoundsCommand : SlashCommand
{
	public override bool Defer => false;

	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("available-sounds")
			.WithDescription("Lists all the sounds that can be played")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("show-server-names")
				.WithDescription("Shows the servers each sound comes from")
				.WithType(ApplicationCommandOptionType.Boolean)
				.WithRequired(false)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		string message = "";

		var showServers = command.Data.Options.SingleOrDefault(o => o.Name == "show-server-names")?.Value as bool?;
		if (showServers is not null and not false)
		{
			var groups = Bot.VoiceChannelConnection.AvailableSounds.GroupBy(s => s.GuildId);
			foreach (var group in groups)
			{
				message += $"### {(group.Key == null ? "Default" : Bot.Client.GetGuild(ulong.Parse(group.Key)).Name)}\n";
				message += $"{String.Join("\n", group.Select(s => $"* {s.Name}"))}\n";
			}
		}
		else
		{
			message = String.Join('\n', Bot.VoiceChannelConnection.AvailableSounds.Select(s => s.Name));
		}

		await command.RespondAsync(message, ephemeral: true);
	}
}