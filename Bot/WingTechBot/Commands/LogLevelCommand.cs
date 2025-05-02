namespace WingTechBot.Commands;

public sealed class LogLevelCommand : SlashCommand
{
	public override bool Admin => true;

	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("log-level")
			.WithDescription("Set the log level of the bot")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("level")
				.WithDescription("The new log level")
				.WithType(ApplicationCommandOptionType.Integer)
				.WithRequired(true)
				.AddChoice("Error", (int)LogSeverity.Error)
				.AddChoice("Warning", (int)LogSeverity.Warning)
				.AddChoice("Info", (int)LogSeverity.Info)
				.AddChoice("Verbose", (int)LogSeverity.Verbose)
				.AddChoice("Debug", (int)LogSeverity.Debug)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.Data.Options.SingleOrDefault(o => o.Name == "level")?.Value is not long level)
			throw new ArgumentException("Provide a log level");

		Bot.Config.LogLevel = (LogSeverity)level;

		await command.FollowupAsync($"Log level set to {Enum.GetName(typeof(LogSeverity), level)}");
	}
}