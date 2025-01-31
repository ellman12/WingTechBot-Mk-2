namespace WingTechBot.Games.GameCommands;

public sealed class StartGameCommand : SlashCommand
{
	protected override SlashCommandBuilder CreateCommand()
	{
		return new SlashCommandBuilder()
			.WithName("start-game")
			.WithDescription("Used to start a game in a new thread")
			.AddOption(new SlashCommandOptionBuilder()
				.WithName("game-name")
				.WithDescription("Name of the game to start")
				.WithType(ApplicationCommandOptionType.String)
				.WithRequired(true)
			);
	}

	public override async Task HandleCommand(SocketSlashCommand command)
	{
		if (command.CommandName != Name)
			return;

		var options = command.Data.Options;
		var gameName = (string)options.First().Value;

		Type game = Bot.GameHandler.AvailableGames.FirstOrDefault(g => String.Equals(g.Name, gameName, StringComparison.CurrentCultureIgnoreCase));
		if (game == null)
		{
			await command.FollowupAsync($"Unknown game: {gameName}");
			return;
		}

		await Bot.GameHandler.CreateGame(command, gameName);
	}
}